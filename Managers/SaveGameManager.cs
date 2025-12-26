using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GuildMaster.Models;
using GuildMaster.Data;

namespace GuildMaster.Managers
{
    public class SaveGameManager
    {
        private const int CURRENT_SAVE_VERSION = 2;
        private const string SAVE_FILE_PREFIX = "savegame";
        private const string SAVE_FILE_EXT = ".json";
        private GameContext context;
        private IStorageService storageService;
        private int currentSlot = 1;
        private GameState? lastLoadedState = null;  // Store last loaded state for post-load initialization

        public SaveGameManager(GameContext gameContext, IStorageService storageService)
        {
            this.context = gameContext;
            this.storageService = storageService;
        }

        private string GetSaveFileName(int slot)
        {
            return $"{SAVE_FILE_PREFIX}{slot}{SAVE_FILE_EXT}";
        }

        private string GetAutosaveFileName()
        {
            return "autosave.json";
        }

        public async Task SaveGameAsync(int? slot = null)
        {
            if (slot.HasValue)
                currentSlot = slot.Value;

            string saveFile = GetSaveFileName(currentSlot);

            try
            {
                var gameState = new GameState();
                gameState.SaveVersion = CURRENT_SAVE_VERSION;
                var player = context.Player;

                // Use helper methods to serialize game state
                SerializePlayerState(player, gameState);
                SerializeNPCStates(gameState);
                gameState.Recruits = SerializeRecruits(player.Recruits);
                gameState.ActivePartyNames = SerializeActiveParty(player.ActiveParty);
                gameState.ActiveQuests = SerializeQuests(player.ActiveQuests);
                SerializeGameplayFlags(player, gameState);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                };

                string json = JsonSerializer.Serialize(gameState, options);

                // Create backup before saving
                if (await storageService.ExistsAsync(saveFile))
                {
                    await storageService.CopyAsync(saveFile, saveFile + ".backup");
                }

                await storageService.WriteTextAsync(saveFile, json);

                // Store metadata for last write time
                var metadata = new { LastWriteTime = DateTime.Now };
                await storageService.WriteTextAsync($"{saveFile}_metadata", JsonSerializer.Serialize(metadata));

                AnsiConsole.MarkupLine($"Game saved to slot {currentSlot}!");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error saving game: {ex.Message}[/]");
            }
        }

        public async Task AutoSaveAsync()
        {
            string saveFile = GetAutosaveFileName();

            try
            {
                var gameState = new GameState();
                gameState.SaveVersion = CURRENT_SAVE_VERSION;
                var player = context.Player;

                // Use helper methods to serialize game state
                SerializePlayerState(player, gameState);
                SerializeNPCStates(gameState);
                gameState.Recruits = SerializeRecruits(player.Recruits);
                gameState.ActivePartyNames = SerializeActiveParty(player.ActiveParty);
                gameState.ActiveQuests = SerializeQuests(player.ActiveQuests);
                SerializeGameplayFlags(player, gameState);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                };

                string json = JsonSerializer.Serialize(gameState, options);
                await storageService.WriteTextAsync(saveFile, json);

                // Store metadata for last write time
                var metadata = new { LastWriteTime = DateTime.Now };
                await storageService.WriteTextAsync($"{saveFile}_metadata", JsonSerializer.Serialize(metadata));

                AnsiConsole.MarkupLine("Game autosaved!");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error autosaving game: {ex.Message}[/]");
            }
        }

        public async Task<bool> LoadGameAsync(int slot = 1)
        {
            currentSlot = slot;
            // Handle autosave slot (slot 4)
            string saveFile = slot == 4 ? GetAutosaveFileName() : GetSaveFileName(slot);

            try
            {
                if (!await storageService.ExistsAsync(saveFile))
                {
                    AnsiConsole.MarkupLine($"No save file found in slot {slot}.");
                    return false;
                }

                string? jsonString = await storageService.ReadTextAsync(saveFile);
                if (jsonString == null)
                {
                    AnsiConsole.MarkupLine($"Error reading save file from slot {slot}.");
                    return false;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                GameState loadedState = JsonSerializer.Deserialize<GameState>(jsonString, options);

                if (loadedState == null)
                {
                    AnsiConsole.MarkupLine("Failed to deserialize save file.");
                    return false;
                }

                // Handle save version migration
                if (loadedState.SaveVersion < CURRENT_SAVE_VERSION)
                {
                    AnsiConsole.MarkupLine($"[yellow]Migrating save from version {loadedState.SaveVersion} to {CURRENT_SAVE_VERSION}...[/]");
                    loadedState = MigrateSaveData(loadedState);
                }

                // Store loaded state for post-initialization (e.g., message restoration)
                lastLoadedState = loadedState;

                // Re-initialize game data
                context.NPCs = NPCData.InitializeNPCs();
                context.Rooms = RoomData.InitializeRooms(context.NPCs);
                context.ItemDescriptions = ItemData.InitializeItemDescriptions();
                context.Effects = EffectData.InitializeEffects();

                // Create new player instance
                context.Player = new Player(loadedState.PlayerName);

                // Apply loaded state
                ApplyLoadedState(loadedState);

                AnsiConsole.MarkupLine($"Save file loaded successfully from slot {slot}!");
                return true;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error loading save: {ex.Message}[/]");

                // Try loading backup
                if (await storageService.ExistsAsync(saveFile + ".backup"))
                {
                    AnsiConsole.MarkupLine("[yellow]Attempting to load backup save...[/]");
                    await storageService.CopyAsync(saveFile + ".backup", saveFile);
                    return await LoadGameAsync(slot);
                }
                return false;
            }
        }

        private GameState MigrateSaveData(GameState oldState)
        {
            // Handle migrations based on version
            if (oldState.SaveVersion == 0 || oldState.SaveVersion == 1)
            {
                // Migration from version 1 to 2
                if (oldState.CurrentDay == 0) oldState.CurrentDay = 1;
                if (string.IsNullOrEmpty(oldState.EquippedWeapon))
                    oldState.EquippedWeapon = "rusty dagger";
                if (oldState.MaxHealth == 0) oldState.MaxHealth = 20;
                if (oldState.Health == 0) oldState.Health = oldState.MaxHealth;
                if (oldState.MaxEnergy == 0) oldState.MaxEnergy = 10;
                if (oldState.Energy == 0) oldState.Energy = oldState.MaxEnergy;
                if (oldState.Speed == 0) oldState.Speed = 1;
                if (oldState.AttackDamage == 0) oldState.AttackDamage = 1;

                // Initialize collections if null
                oldState.Recruits ??= new List<SavedRecruit>();
                oldState.ActiveQuests ??= new List<SavedQuest>();
                oldState.RemovedNPCs ??= new Dictionary<int, List<string>>();
                oldState.ActivePartyNames ??= new List<string>();
            }

            oldState.SaveVersion = CURRENT_SAVE_VERSION;
            return oldState;
        }

        private void ApplyLoadedState(GameState state)
        {
            var player = context.Player;

            // Basic info
            player.CurrentRoom = state.CurrentRoom > 0 ? state.CurrentRoom : 1;
            player.Inventory = state.PlayerInventory ?? new List<string>();
            player.TakenItems = new HashSet<string>(state.TakenItems ?? new HashSet<string>());
            player.ExaminedItems = new HashSet<string>(state.ExaminedItems ?? new HashSet<string>());

            // Stats
            player.Health = state.Health > 0 ? state.Health : 20;
            player.MaxHealth = state.MaxHealth > 0 ? state.MaxHealth : 20;
            player.Energy = state.Energy >= 0 ? state.Energy : 10;
            player.MaxEnergy = state.MaxEnergy > 0 ? state.MaxEnergy : 10;
            player.Gold = state.Gold >= 0 ? state.Gold : 0;
            player.AttackDamage = state.AttackDamage > 0 ? state.AttackDamage : 1;
            player.Defense = state.Defense >= 0 ? state.Defense : 0;
            player.Speed = state.Speed > 0 ? state.Speed : 1;
            player.Level = state.Level > 0 ? state.Level : 1;
            player.Experience = state.Experience >= 0 ? state.Experience : 0;
            player.ExperienceToNextLevel = state.ExperienceToNextLevel > 0 ? state.ExperienceToNextLevel : 100;
            player.AutoCombatEnabled = state.AutoCombatEnabled;
            player.TutorialsEnabled = state.TutorialsEnabled;
            player.GoreEnabled = state.GoreEnabled;
            player.DebugLogsEnabled = state.DebugLogsEnabled;

            // Load equipment - try new format first, fall back to old format
            if (!string.IsNullOrEmpty(state.EquippedWeaponName))
            {
                player.EquippedWeapon = EquipmentData.GetEquipment(state.EquippedWeaponName);
            }
            else if (!string.IsNullOrEmpty(state.EquippedWeapon))
            {
                // Backward compatibility - old saves had string weapon names
                player.EquippedWeapon = EquipmentData.GetEquipment(state.EquippedWeapon);
            }
            else
            {
                player.EquippedWeapon = EquipmentData.GetEquipment("rusty dagger");
            }

            // Load other equipment slots (only in new format)
            if (!string.IsNullOrEmpty(state.EquippedArmorName))
                player.EquippedArmor = EquipmentData.GetEquipment(state.EquippedArmorName);
            if (!string.IsNullOrEmpty(state.EquippedHelmName))
                player.EquippedHelm = EquipmentData.GetEquipment(state.EquippedHelmName);
            if (!string.IsNullOrEmpty(state.EquippedRingName))
                player.EquippedRing = EquipmentData.GetEquipment(state.EquippedRingName);

            if (!string.IsNullOrEmpty(state.PlayerClass))


                if (!string.IsNullOrEmpty(state.PlayerClass))
            {
                switch (state.PlayerClass)
                {
                    case "Legionnaire":
                        player.Class = new Legionnaire();
                        break;
                    case "Venator":
                        player.Class = new Venator();
                        break;
                    case "Oracle":
                        player.Class = new Oracle();
                        break;
                }
            }

            // Time
            player.CurrentDay = state.CurrentDay > 0 ? state.CurrentDay : 1;
            player.CurrentHour = state.CurrentHour >= 0 ? state.CurrentHour : 8.0f;

            // Initialize note text with player name
            context.NoteText = GuildMaster.Data.NarrativeData.GenerateWelcomeNote(player.Name, player.Class.Name);

            // Restore NPC dialogue states
            if (state.NPCDialogueStates != null)
            {
                foreach (var kvp in state.NPCDialogueStates)
                {
                    if (context.NPCs.ContainsKey(kvp.Key))
                    {
                        context.NPCs[kvp.Key].CurrentDialogueNode = kvp.Value;
                    }
                }
            }

            // Restore NPCs in rooms (only those not defeated)
            if (state.RemovedNPCs != null)
            {
                // Clear all NPCs from rooms first
                foreach (var room in context.Rooms.Values)
                {
                    room.NPCs.Clear();
                }

                // Add back only the ones that weren't defeated
                foreach (var kvp in state.RemovedNPCs)
                {
                    int roomId = kvp.Key;
                    List<string> npcNames = kvp.Value;

                    if (context.Rooms.ContainsKey(roomId))
                    {
                        foreach (var npcName in npcNames)
                        {
                            if (context.NPCs.ContainsKey(npcName))
                            {
                                context.Rooms[roomId].NPCs.Add(context.NPCs[npcName]);
                            }
                        }
                    }
                }
            }

            // Restore recruits
            player.Recruits = new List<Recruit>();
            if (state.Recruits != null)
            {
                foreach (var savedRecruit in state.Recruits)
                {
                    var recruit = new Recruit(savedRecruit.Name, savedRecruit.Class, savedRecruit.RecruitedDay);
                    recruit.Health = savedRecruit.Health;
                    recruit.MaxHealth = savedRecruit.MaxHealth;
                    recruit.Energy = savedRecruit.Energy;
                    recruit.MaxEnergy = savedRecruit.MaxEnergy;
                    recruit.AttackDamage = savedRecruit.AttackDamage > 0 ? savedRecruit.AttackDamage : recruit.AttackDamage;
                    recruit.Defense = savedRecruit.Defense;
                    recruit.Speed = savedRecruit.Speed;

                    // Load equipment - try new format first, fall back to old format
                    if (!string.IsNullOrEmpty(savedRecruit.EquippedWeaponName))
                    {
                        recruit.EquippedWeapon = EquipmentData.GetEquipment(savedRecruit.EquippedWeaponName);
                    }
                    else if (!string.IsNullOrEmpty(savedRecruit.EquippedWeapon))
                    {
                        // Backward compatibility
                        recruit.EquippedWeapon = EquipmentData.GetEquipment(savedRecruit.EquippedWeapon);
                    }
                    else
                    {
                        recruit.EquippedWeapon = EquipmentData.GetEquipment("rusty dagger");
                    }

                    // Load other equipment slots
                    if (!string.IsNullOrEmpty(savedRecruit.EquippedArmorName))
                        recruit.EquippedArmor = EquipmentData.GetEquipment(savedRecruit.EquippedArmorName);
                    if (!string.IsNullOrEmpty(savedRecruit.EquippedHelmName))
                        recruit.EquippedHelm = EquipmentData.GetEquipment(savedRecruit.EquippedHelmName);
                    if (!string.IsNullOrEmpty(savedRecruit.EquippedRingName))
                        recruit.EquippedRing = EquipmentData.GetEquipment(savedRecruit.EquippedRingName);

                    recruit.Level = savedRecruit.Level > 0 ? savedRecruit.Level : 1;
                    recruit.Experience = savedRecruit.Experience;
                    recruit.ExperienceToNextLevel = savedRecruit.ExperienceToNextLevel > 0 ? savedRecruit.ExperienceToNextLevel : 100;
                    recruit.IsOnQuest = savedRecruit.IsOnQuest;
                    recruit.IsResting = savedRecruit.IsResting;
                    recruit.RestUntil = savedRecruit.RestUntil;
                    recruit.RestUntilDay = savedRecruit.RestUntilDay;

                    player.Recruits.Add(recruit);
                }
            }

            // Restore active party
            player.ActiveParty = new List<Recruit>();
            if (state.ActivePartyNames != null)
            {
                foreach (var name in state.ActivePartyNames)
                {
                    var recruit = player.Recruits.FirstOrDefault(r => r.Name == name);
                    if (recruit != null)
                    {
                        player.ActiveParty.Add(recruit);
                    }
                }
            }

            // Restore quests
            player.ActiveQuests = new List<Quest>();
            if (state.ActiveQuests != null)
            {
                foreach (var savedQuest in state.ActiveQuests)
                {
                    var quest = new Quest
                    {
                        Id = savedQuest.Id,
                        Name = savedQuest.Name,
                        Description = savedQuest.Description,
                        Difficulty = savedQuest.Difficulty,
                        StartDay = savedQuest.StartDay,
                        StartTime = savedQuest.StartTime,
                        Duration = savedQuest.Duration,
                        MinGold = savedQuest.MinGold,
                        MaxGold = savedQuest.MaxGold,
                        BaseSuccessChance = savedQuest.BaseSuccessChance,
                        BaseExperienceReward = savedQuest.BaseExperienceReward,
                        IsActive = savedQuest.IsActive,
                        IsComplete = savedQuest.IsComplete,
                        WasSuccessful = savedQuest.WasSuccessful,
                        ItemRewards = savedQuest.ItemRewards ?? new Dictionary<string, int>(),
                        PotentialRecruit = savedQuest.PotentialRecruit
                    };

                    // Reconnect quest to recruit
                    if (!string.IsNullOrEmpty(savedQuest.AssignedRecruitName))
                    {
                        quest.AssignedRecruit = player.Recruits.FirstOrDefault(r => r.Name == savedQuest.AssignedRecruitName);
                    }

                    player.ActiveQuests.Add(quest);
                }
            }

            // Restore completed quest IDs
            player.CompletedQuestIds = state.CompletedQuestIds ?? new List<string>();

            // Restore quest flags
            player.QuestFlags = state.QuestFlags ?? new Dictionary<string, bool>();

            // Restore triggered event IDs (if EventManager is available)
            if (ProgramStatics.eventManager != null && state.TriggeredEventIds != null)
            {
                ProgramStatics.eventManager.SetTriggeredEvents(state.TriggeredEventIds);
            }

            // Remove taken items from rooms
            foreach (string takenItem in player.TakenItems)
            {
                if (takenItem.StartsWith("room") && takenItem.Contains("_"))
                {
                    string[] parts = takenItem.Split('_');
                    string roomNumStr = parts[0].Replace("room", "");
                    if (int.TryParse(roomNumStr, out int roomNum) && context.Rooms.ContainsKey(roomNum))
                    {
                        string itemName = string.Join("_", parts.Skip(1));
                        context.Rooms[roomNum].Items.Remove(itemName);
                    }
                }
            }

            // Restore container contents that were revealed
            foreach (string examinedItem in player.ExaminedItems)
            {
                if (examinedItem.StartsWith("room") && examinedItem.Contains("_"))
                {
                    string[] parts = examinedItem.Split('_');
                    if (int.TryParse(parts[0].Replace("room", ""), out int roomNum) && context.Rooms.ContainsKey(roomNum))
                    {
                        string containerName = string.Join("_", parts.Skip(1));

                        if (context.ItemDescriptions.ContainsKey(roomNum) &&
                            context.ItemDescriptions[roomNum].ContainsKey(containerName) &&
                            context.ItemDescriptions[roomNum][containerName].IsContainer)
                        {
                            var container = context.ItemDescriptions[roomNum][containerName];
                            foreach (string item in container.Contents)
                            {
                                if (!player.TakenItems.Contains($"room{roomNum}_{item}") &&
                                    !context.Rooms[roomNum].Items.Contains(item))
                                {
                                    context.Rooms[roomNum].Items.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            // Restore shown messages
            if (state.ShownMessages != null && ProgramStatics.messageManager != null)
            {
                ProgramStatics.messageManager.SetShownMessages(state.ShownMessages);
            }

            // Restore milestone tracking
            context.TotalRecruitsEver = state.TotalRecruitsEver;
            context.CompletedMilestones = state.CompletedMilestones ?? new HashSet<string>();
            context.RoomStateOverrides = state.RoomStateOverrides ?? new Dictionary<int, string>();
        }

        public async Task<bool> SaveExistsAsync(int slot)
        {
            return await storageService.ExistsAsync(GetSaveFileName(slot));
        }

        public async Task<bool> SaveExistsAsync()
        {
            // Check if ANY save file exists (for backward compatibility)
            return await SaveExistsAsync(1) || await SaveExistsAsync(2) || await SaveExistsAsync(3) ||
                   await storageService.ExistsAsync("savegame.json"); // Check old format too
        }

        public int GetCurrentSlot()
        {
            return currentSlot;
        }

        public GameState? GetLastLoadedState()
        {
            return lastLoadedState;
        }

        public class SaveSlotInfo
        {
            public bool Exists { get; set; }
            public string CharacterName { get; set; } = "Empty";
            public int Day { get; set; }
            public int Recruits { get; set; }
            public int Gold { get; set; }
            public DateTime SaveTime { get; set; }
        }

        public async Task<SaveSlotInfo> GetSlotInfoAsync(int slot)
        {
            var info = new SaveSlotInfo();
            string saveFile = GetSaveFileName(slot);

            if (!await storageService.ExistsAsync(saveFile))
            {
                info.Exists = false;
                return info;
            }

            try
            {
                info.Exists = true;
                var saveTime = await storageService.GetLastWriteTimeAsync(saveFile);
                info.SaveTime = saveTime ?? DateTime.MinValue;

                // Quick read just the info we need
                string? json = await storageService.ReadTextAsync(saveFile);
                if (json != null)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var state = JsonSerializer.Deserialize<GameState>(json, options);

                    if (state != null)
                    {
                        info.CharacterName = state.PlayerName ?? "Unknown";
                        info.Day = state.CurrentDay;
                        info.Recruits = state.Recruits?.Count ?? 0;
                        info.Gold = state.Gold;
                    }
                }
            }
            catch
            {
                // If we can't read it, just mark it as corrupted
                info.CharacterName = "Corrupted Save";
            }

            return info;
        }

        public async Task<SaveSlotInfo> GetAutosaveSlotInfoAsync()
        {
            var info = new SaveSlotInfo();
            string saveFile = GetAutosaveFileName();

            if (!await storageService.ExistsAsync(saveFile))
            {
                info.Exists = false;
                return info;
            }

            try
            {
                info.Exists = true;
                var saveTime = await storageService.GetLastWriteTimeAsync(saveFile);
                info.SaveTime = saveTime ?? DateTime.MinValue;

                // Quick read just the info we need
                string? json = await storageService.ReadTextAsync(saveFile);
                if (json != null)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var state = JsonSerializer.Deserialize<GameState>(json, options);

                    if (state != null)
                    {
                        info.CharacterName = state.PlayerName ?? "Unknown";
                        info.Day = state.CurrentDay;
                        info.Recruits = state.Recruits?.Count ?? 0;
                        info.Gold = state.Gold;
                    }
                }
            }
            catch
            {
                // If we can't read it, just mark it as corrupted
                info.CharacterName = "Corrupted Save";
            }

            return info;
        }

        public async Task DisplaySaveMenuAsync()
        {
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#FA935F]═══════════════════════════════════════════════════════════════════[/]");
            AnsiConsole.MarkupLine("[#FA935F]                            SAVE GAME                              [/]");
            AnsiConsole.MarkupLine("[#FA935F]═══════════════════════════════════════════════════════════════════[/]");
            AnsiConsole.MarkupLine("");

            for (int i = 1; i <= 3; i++)
            {
                var info = await GetSlotInfoAsync(i);

                if (info.Exists)
                {
                    AnsiConsole.MarkupLine($"[#FFD700]{i}.[/] [#00FF00]{info.CharacterName}[/]");
                    AnsiConsole.MarkupLine($"   Day {info.Day} | Recruits: {info.Recruits}/10 | Gold: {info.Gold}");
                    AnsiConsole.MarkupLine($"   [dim]Saved: {info.SaveTime:yyyy-MM-dd HH:mm}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[#FFD700]{i}.[/] [dim]Empty Slot[/]");
                }
                AnsiConsole.MarkupLine("");
            }

            AnsiConsole.MarkupLine("[#808080]0. Cancel[/]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        private int? pendingSaveSlot = null;

        public async Task<bool> ProcessSaveInputAsync(string input)
        {
            // If we're waiting for overwrite confirmation
            if (pendingSaveSlot.HasValue)
            {
                if (input == "y" || input == "yes")
                {
                    await SaveGameAsync(pendingSaveSlot.Value);
                    pendingSaveSlot = null;
                    return true;
                }
                else
                {
                    AnsiConsole.MarkupLine("\nSave cancelled.");
                    pendingSaveSlot = null;
                    return true;
                }
            }

            // Otherwise, processing slot selection
            if (!int.TryParse(input, out int slot))
            {
                AnsiConsole.MarkupLine("\n[dim]Invalid choice. Please try again.[/]");
                return false;
            }

            if (slot == 0)
            {
                return true;
            }

            if (slot >= 1 && slot <= 3)
            {
                var info = await GetSlotInfoAsync(slot);
                if (info.Exists)
                {
                    pendingSaveSlot = slot;
                    AnsiConsole.MarkupLine($"\nOverwrite save for {info.CharacterName}? (y/n): ");
                    return false; // Stay in menu, waiting for confirmation
                }
                else
                {
                    await SaveGameAsync(slot);
                    return true;
                }
            }
            else
            {
                AnsiConsole.MarkupLine("\n[red]Invalid slot number.[/]");
                return false;
            }
        }

        public async Task<bool> DisplayLoadMenuAsync()
        {
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#75C8FF]═══════════════════════════════════════════════════════════════════[/]");
            AnsiConsole.MarkupLine("[#75C8FF]                            LOAD GAME                              [/]");
            AnsiConsole.MarkupLine("[#75C8FF]═══════════════════════════════════════════════════════════════════[/]");
            AnsiConsole.MarkupLine("");

            bool anySaves = false;

            for (int i = 1; i <= 3; i++)
            {
                var info = await GetSlotInfoAsync(i);

                if (info.Exists)
                {
                    anySaves = true;
                    AnsiConsole.MarkupLine($"[#FFD700]{i}.[/] [#00FF00]{info.CharacterName}[/]");
                    AnsiConsole.MarkupLine($"   Day {info.Day} | Recruits: {info.Recruits}/10 | Gold: {info.Gold}");
                    AnsiConsole.MarkupLine($"   [dim]Saved: {info.SaveTime:yyyy-MM-dd HH:mm}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[#FFD700]{i}.[/] [dim]Empty Slot[/]");
                }
                AnsiConsole.MarkupLine("");
            }

            // Show autosave slot with separator
            AnsiConsole.MarkupLine("[dim]─────────────────────────────────────────────────────────────────[/]");
            AnsiConsole.MarkupLine("");

            var autosaveInfo = await GetAutosaveSlotInfoAsync();
            if (autosaveInfo.Exists)
            {
                anySaves = true;
                AnsiConsole.MarkupLine($"[#FFD700]4.[/] [#00FF00]Autosave[/]");
                AnsiConsole.MarkupLine($"   {autosaveInfo.CharacterName}");
                AnsiConsole.MarkupLine($"   Day {autosaveInfo.Day} | Recruits: {autosaveInfo.Recruits}/10 | Gold: {autosaveInfo.Gold}");
                AnsiConsole.MarkupLine($"   [dim]Saved: {autosaveInfo.SaveTime:yyyy-MM-dd HH:mm}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[#FFD700]4.[/] [dim]Autosave (Empty)[/]");
            }
            AnsiConsole.MarkupLine("");

            if (!anySaves)
            {
                AnsiConsole.MarkupLine("[yellow]No saved games found.[/]");
                AnsiConsole.MarkupLine("");
                return false;
            }

            AnsiConsole.MarkupLine("[#808080]0. Cancel[/]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
            return true;
        }

        public async Task<bool> ProcessLoadInputAsync(string input)
        {
            if (!int.TryParse(input, out int slot))
            {
                AnsiConsole.MarkupLine("\n[dim]Invalid choice. Please try again.[/]");
                return false;
            }

            if (slot == 0)
            {
                return true;
            }

            if (slot >= 1 && slot <= 3)
            {
                var slotInfo = await GetSlotInfoAsync(slot);
                if (slotInfo.Exists)
                {
                    if (await LoadGameAsync(slot))
                    {
                        AnsiConsole.MarkupLine("\n[#00FF00]Game loaded successfully![/]");
                        return true;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("\n[red]Failed to load game.[/]");
                        return false;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[red]That slot is empty![/]");
                    return false;
                }
            }
            else if (slot == 4) // Autosave slot
            {
                var slotInfo = await GetAutosaveSlotInfoAsync();
                if (slotInfo.Exists)
                {
                    if (await LoadGameAsync(4))
                    {
                        AnsiConsole.MarkupLine("\n[#00FF00]Autosave loaded successfully![/]");
                        return true;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("\n[red]Failed to load autosave.[/]");
                        return false;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[red]Autosave slot is empty![/]");
                    return false;
                }
            }
            else
            {
                AnsiConsole.MarkupLine("\n[red]Invalid slot number.[/]");
                return false;
            }
        }

        // ============================================================================
        // SERIALIZATION HELPER METHODS
        // ============================================================================

        /// <summary>
        /// Serializes player state including basics, stats, equipment, settings, and time
        /// </summary>
        private void SerializePlayerState(Player player, GameState gameState)
        {
            // Player basics
            gameState.PlayerName = player.Name;
            gameState.CurrentRoom = player.CurrentRoom;
            gameState.PlayerInventory = player.Inventory;
            gameState.TakenItems = player.TakenItems;
            gameState.ExaminedItems = player.ExaminedItems;

            // Player stats
            gameState.Health = player.Health;
            gameState.MaxHealth = player.MaxHealth;
            gameState.Energy = player.Energy;
            gameState.MaxEnergy = player.MaxEnergy;
            gameState.Gold = player.Gold;
            gameState.AttackDamage = player.AttackDamage;
            gameState.Defense = player.Defense;
            gameState.Speed = player.Speed;
            gameState.EquippedWeaponName = player.EquippedWeapon?.Name.ToLower();
            gameState.EquippedArmorName = player.EquippedArmor?.Name.ToLower();
            gameState.EquippedHelmName = player.EquippedHelm?.Name.ToLower();
            gameState.EquippedRingName = player.EquippedRing?.Name.ToLower();
            gameState.PlayerClass = player.Class?.Name ?? "Fighter";
            gameState.Level = player.Level;
            gameState.Experience = player.Experience;
            gameState.ExperienceToNextLevel = player.ExperienceToNextLevel;
            gameState.AutoCombatEnabled = player.AutoCombatEnabled;
            gameState.TutorialsEnabled = player.TutorialsEnabled;
            gameState.GoreEnabled = player.GoreEnabled;
            gameState.DebugLogsEnabled = player.DebugLogsEnabled;

            // Milestone tracking
            gameState.TotalRecruitsEver = context.TotalRecruitsEver;
            gameState.CompletedMilestones = context.CompletedMilestones ?? new HashSet<string>();
            gameState.RoomStateOverrides = context.RoomStateOverrides ?? new Dictionary<int, string>();

            // Time
            gameState.CurrentDay = player.CurrentDay;
            gameState.CurrentHour = player.CurrentHour;
        }

        /// <summary>
        /// Serializes NPC dialogue states and tracks which NPCs are still in rooms
        /// </summary>
        private void SerializeNPCStates(GameState gameState)
        {
            // NPCs
            foreach (var npc in context.NPCs.Values)
            {
                gameState.NPCDialogueStates[npc.Name] = npc.CurrentDialogueNode;
            }

            // Track which NPCs are still in rooms (not defeated)
            foreach (var room in context.Rooms.Values)
            {
                if (room.NPCs.Count > 0)
                {
                    if (!gameState.RemovedNPCs.ContainsKey(room.NumericId))
                    {
                        gameState.RemovedNPCs[room.NumericId] = new List<string>();
                    }
                    foreach (var npc in room.NPCs)
                    {
                        gameState.RemovedNPCs[room.NumericId].Add(npc.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Serializes recruit list to SavedRecruit format
        /// </summary>
        private List<SavedRecruit> SerializeRecruits(List<Recruit> recruits)
        {
            var savedRecruits = new List<SavedRecruit>();
            foreach (var recruit in recruits)
            {
                var savedRecruit = new SavedRecruit
                {
                    Name = recruit.Name,
                    Class = recruit.Class?.Name ?? "Fighter",
                    RecruitedDay = recruit.RecruitedDay,
                    Health = recruit.Health,
                    MaxHealth = recruit.MaxHealth,
                    Energy = recruit.Energy,
                    MaxEnergy = recruit.MaxEnergy,
                    AttackDamage = recruit.AttackDamage,
                    Defense = recruit.Defense,
                    Speed = recruit.Speed,
                    EquippedWeaponName = recruit.EquippedWeapon?.Name.ToLower(),
                    EquippedArmorName = recruit.EquippedArmor?.Name.ToLower(),
                    EquippedHelmName = recruit.EquippedHelm?.Name.ToLower(),
                    EquippedRingName = recruit.EquippedRing?.Name.ToLower(),
                    Level = recruit.Level,
                    Experience = recruit.Experience,
                    ExperienceToNextLevel = recruit.ExperienceToNextLevel,
                    IsOnQuest = recruit.IsOnQuest,
                    IsResting = recruit.IsResting,
                    RestUntil = recruit.RestUntil,
                    RestUntilDay = recruit.RestUntilDay
                };
                savedRecruits.Add(savedRecruit);
            }
            return savedRecruits;
        }

        /// <summary>
        /// Serializes active party to list of recruit names
        /// </summary>
        private List<string> SerializeActiveParty(List<Recruit> activeParty)
        {
            var partyNames = new List<string>();
            foreach (var member in activeParty)
            {
                partyNames.Add(member.Name);
            }
            return partyNames;
        }

        /// <summary>
        /// Serializes quest list to SavedQuest format
        /// </summary>
        private List<SavedQuest> SerializeQuests(List<Quest> quests)
        {
            var savedQuests = new List<SavedQuest>();
            foreach (var quest in quests)
            {
                var savedQuest = new SavedQuest
                {
                    Id = quest.Id,
                    Name = quest.Name,
                    Description = quest.Description,
                    Difficulty = quest.Difficulty,
                    AssignedRecruitName = quest.AssignedRecruit?.Name,
                    StartDay = quest.StartDay,
                    StartTime = quest.StartTime,
                    Duration = quest.Duration,
                    MinGold = quest.MinGold,
                    MaxGold = quest.MaxGold,
                    BaseSuccessChance = quest.BaseSuccessChance,
                    BaseExperienceReward = quest.BaseExperienceReward,
                    IsActive = quest.IsActive,
                    IsComplete = quest.IsComplete,
                    WasSuccessful = quest.WasSuccessful,
                    ItemRewards = quest.ItemRewards,
                    PotentialRecruit = quest.PotentialRecruit
                };
                savedQuests.Add(savedQuest);
            }
            return savedQuests;
        }

        /// <summary>
        /// Serializes gameplay flags including completed quests, quest flags, events, and messages
        /// </summary>
        private void SerializeGameplayFlags(Player player, GameState gameState)
        {
            // Save completed quest IDs
            gameState.CompletedQuestIds = player.CompletedQuestIds;

            // Save quest flags
            gameState.QuestFlags = player.QuestFlags;

            // Save triggered event IDs (if EventManager is available)
            if (ProgramStatics.eventManager != null)
            {
                gameState.TriggeredEventIds = ProgramStatics.eventManager.GetTriggeredEvents();
            }

            // Save shown messages
            if (ProgramStatics.messageManager != null)
            {
                gameState.ShownMessages = ProgramStatics.messageManager.GetShownMessages();
            }
        }
    }
}