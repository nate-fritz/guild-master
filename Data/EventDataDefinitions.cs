using System.Collections.Generic;
using GuildMaster.Models;
using GuildMaster.Managers;

namespace GuildMaster.Data
{
    /// <summary>
    /// Contains sample event definitions demonstrating the event system capabilities
    /// </summary>
    public static class EventDataDefinitions
    {
        /// <summary>
        /// Returns a list of all defined events
        /// Call this from EventManager.LoadEvents() to load events into the game
        /// </summary>
        public static List<EventData> GetAllEvents()
        {
            var events = new List<EventData>();

            // Add all event definitions
            events.Add(CreateGuildHallWelcomeEvent());
            events.Add(CreateTenthRecruitCelebrationEvent());
            events.Add(CreateGuildCouncilMeetingEvent());
            events.Add(CreateAevoriaArrivalEvent());
            events.Add(CreateCelebrationStartEvent());
            events.Add(CreateAssassinationEvent());
            events.Add(CreateAftermathEvent());
            events.Add(CreateActTwoIntroEvent());
            events.Add(CreateFarmBanditAttackEvent());

            return events;
        }

        /// <summary>
        /// Example: Simple welcome event when first entering the Guild Hall Common Area
        /// </summary>
        private static EventData CreateGuildHallWelcomeEvent()
        {
            return new EventData("guild_hall_welcome", 4)  // Room 4 is Common Area
            {
                Priority = 10,
                IsOneTime = true,
                DialogueTreeId = "guild_hall_welcome_dialogue",  // Reference to dialogue tree
                Conditions = new List<EventCondition>
                {
                    // Triggers only on first visit to this room
                    new EventCondition(ConditionType.FirstVisit, "guild_hall_welcome", true)
                },
                Actions = new List<EventAction>
                {
                    // Set a quest flag to track that we've been welcomed
                    new EventAction(ActionType.SetQuestFlag)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "flagId", "guild_hall_visited" },
                            { "value", true }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Example: Celebration event when recruiting the 10th guild member
        /// Demonstrates recruit count condition and multiple actions
        /// </summary>
        private static EventData CreateTenthRecruitCelebrationEvent()
        {
            return new EventData("tenth_recruit_celebration", 4)  // Room 4 is Common Area
            {
                Priority = 100,
                IsOneTime = true,
                DialogueTreeId = "",  // Could add dialogue here for a celebration scene
                Conditions = new List<EventCondition>
                {
                    // Only trigger when player has exactly 10 recruits (or more)
                    new EventCondition(ConditionType.MinRecruitCount, "", true, 10),
                    // And hasn't triggered this event before
                    new EventCondition(ConditionType.FirstVisit, "tenth_recruit_celebration", true)
                },
                Actions = new List<EventAction>
                {
                    // Grant bonus gold for achievement
                    new EventAction(ActionType.GrantGold)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "amount", 100 }
                        }
                    },
                    // Set quest flag
                    new EventAction(ActionType.SetQuestFlag)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "flagId", "legendary_guild_achieved" },
                            { "value", true }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Guild Council Meeting: Quintus and Caelia discuss the cult threat in the guild study
        /// Triggers when player enters guild study after Quintus/Caelia examination timer completes
        /// </summary>
        private static EventData CreateGuildCouncilMeetingEvent()
        {
            return new EventData("guild_council_meeting", 3)  // Room 3 is Guild Hall Study
            {
                Priority = 200,
                IsOneTime = true,
                DialogueTreeId = "guild_council_meeting_dialogue",
                Conditions = new List<EventCondition>
                {
                    // Requires quest flag from timer completion
                    new EventCondition(ConditionType.QuestFlagSet, "guild_council_ready", true),
                    // First visit to study after flag is set
                    new EventCondition(ConditionType.FirstVisit, "guild_council_meeting", true)
                },
                Actions = new List<EventAction>
                {
                    // Spawn Quintus in the guild study
                    new EventAction(ActionType.SpawnNPC)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "npcName", "Senator Quintus" },
                            { "roomId", 3 }
                        }
                    },
                    // Spawn Caelia in the guild study
                    new EventAction(ActionType.SpawnNPC)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "npcName", "Caelia" },
                            { "roomId", 3 }
                        }
                    }
                    // Note: ForceTravel will be handled at the end of dialogue, not here
                }
            };
        }

        /// <summary>
        /// Aevoria Arrival: Sets quest flag when arriving at Imperial Villa for the first time
        /// Used to track arrival time for celebration countdown
        /// </summary>
        private static EventData CreateAevoriaArrivalEvent()
        {
            return new EventData("aevoria_arrival", 200)  // Room 200 - Imperial Villa Grand Hall
            {
                Priority = 300,
                IsOneTime = true,
                DialogueTreeId = "",  // No dialogue, just sets flags
                Conditions = new List<EventCondition>
                {
                    // First visit to Aevoria
                    new EventCondition(ConditionType.FirstVisit, "aevoria_arrival", true)
                },
                Actions = new List<EventAction>
                {
                    // Set flag indicating player is in Aevoria
                    new EventAction(ActionType.SetQuestFlag)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "flagId", "in_aevoria_villa" },
                            { "value", true }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Celebration Start: Triggers when player has been warned emperor and celebration time arrives
        /// Moves player to Colosseum for the anniversary games
        /// </summary>
        private static EventData CreateCelebrationStartEvent()
        {
            return new EventData("celebration_start", 202)  // Triggers in guest quarters when player wakes
            {
                Priority = 250,
                IsOneTime = true,
                DialogueTreeId = "celebration_start_dialogue",
                Conditions = new List<EventCondition>
                {
                    // Player warned the emperor
                    new EventCondition(ConditionType.QuestFlagSet, "emperor_warned", true),
                    // Celebration is ready to start
                    new EventCondition(ConditionType.QuestFlagSet, "celebration_ready", true),
                    // First time triggering this event
                    new EventCondition(ConditionType.FirstVisit, "celebration_start", true)
                },
                Actions = new List<EventAction>
                {
                    // Remove the villa flag
                    new EventAction(ActionType.SetQuestFlag)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "flagId", "in_aevoria_villa" },
                            { "value", false }
                        }
                    }
                    // Note: ForceTravel to colosseum happens in dialogue
                }
            };
        }

        /// <summary>
        /// Assassination: Player witnesses Emperor's assassination at the Colosseum
        /// Triggers when entering the Emperor's Seat Box
        /// </summary>
        private static EventData CreateAssassinationEvent()
        {
            return new EventData("aevoria_assassination", 223)  // Room 223 - Emperor's Seat Box
            {
                Priority = 100,
                IsOneTime = true,
                DialogueTreeId = "assassination_scene",
                Conditions = new List<EventCondition>
                {
                    // Player is at the celebration
                    new EventCondition(ConditionType.QuestFlagSet, "celebration_ready", true),
                    // First time visiting this room
                    new EventCondition(ConditionType.FirstVisit, "aevoria_assassination", true)
                },
                Actions = new List<EventAction>
                {
                    // Spawn the Imperial Assassin in the room
                    new EventAction(ActionType.SpawnNPC)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "npcName", "Imperial Assassin" },
                            { "roomId", 223 }
                        }
                    },
                    // Set quest flag indicating assassination happened
                    new EventAction(ActionType.SetQuestFlag)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "flagId", "certius_assassinated" },
                            { "value", true }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Aftermath: Quintus and Caelia react to the Emperor's death
        /// Player learns about the new emperor and returns to guild hall
        /// </summary>
        private static EventData CreateAftermathEvent()
        {
            return new EventData("assassination_aftermath", 223)  // Room 223 - Emperor's Seat Box
            {
                Priority = 90,
                IsOneTime = true,
                DialogueTreeId = "aftermath_dialogue",
                Conditions = new List<EventCondition>
                {
                    // Assassination happened
                    new EventCondition(ConditionType.QuestFlagSet, "certius_assassinated", true),
                    // Player has the assassin's dagger (meaning they won the fight)
                    new EventCondition(ConditionType.HasItem, "emperor's blood dagger", true),
                    // First time triggering aftermath
                    new EventCondition(ConditionType.FirstVisit, "assassination_aftermath", true)
                },
                Actions = new List<EventAction>
                {
                    // Set Act I complete flag (enables guild quest system)
                    new EventAction(ActionType.SetQuestFlag)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "flagId", "act_1_complete" },
                            { "value", true }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Act II Intro: Quintus appears in guild study to explain the ongoing fight
        /// Triggers after player returns from Aevoria
        /// </summary>
        private static EventData CreateActTwoIntroEvent()
        {
            return new EventData("act_two_intro", 3)  // Room 3 - Guild Hall Study
            {
                Priority = 80,
                IsOneTime = true,
                DialogueTreeId = "act_two_intro_dialogue",
                Conditions = new List<EventCondition>
                {
                    // Act I is complete
                    new EventCondition(ConditionType.QuestFlagSet, "act_1_complete", true),
                    // First visit to study after Act I
                    new EventCondition(ConditionType.FirstVisit, "act_two_intro", true)
                },
                Actions = new List<EventAction>
                {
                    // Spawn Quintus in the study
                    new EventAction(ActionType.SpawnNPC)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "npcName", "Senator Quintus" },
                            { "roomId", 3 }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Example: Event that grants an item when player has enough gold
        /// </summary>
        private static EventData CreateMerchantGiftEvent()
        {
            return new EventData("merchant_gift", 10)  // Example room ID
            {
                Priority = 50,
                IsOneTime = true,
                DialogueTreeId = "",
                Conditions = new List<EventCondition>
                {
                    // Requires at least 500 gold
                    new EventCondition(ConditionType.MinGold, "", true, 500),
                    // First visit to this room with enough gold
                    new EventCondition(ConditionType.FirstVisit, "merchant_gift", true)
                },
                Actions = new List<EventAction>
                {
                    // Grant a special item
                    new EventAction(ActionType.GrantItem)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "itemId", "merchant's token" }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Example: Demonstrates how to register event dialogue trees
        /// Call this from GameEngine after DialogueManager is created
        /// </summary>
        public static void RegisterEventDialogueTrees(DialogueManager dialogueManager)
        {
            // Register guild hall welcome dialogue
            var guildHallWelcomeDialogue = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Text = "As you step into the common area of the guild hall for the first time, you pause to take it in.<br><br>It is far too large a space for just one person, but you briefly imagine what this place felt like when it was full of brothers and sisters in arms, united in a common purpose.<br><br>You had never planned for anything like this, but the idea suddenly feels like one worth working towards.",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "I'll be back soon..  and not alone.",
                            nextNodeID = "end"
                        }
                    }
                },
                ["end"] = new DialogueNode
                {
                    Text = "You hang on to this feeling for just a moment longer, then get back to the task at hand.",
                    Choices = new List<DialogueNode.Choice>() // No choices - dialogue ends
                }
            };
            dialogueManager.RegisterEventDialogue("guild_hall_welcome_dialogue", guildHallWelcomeDialogue);

            // Register guild council meeting dialogue
            var guildCouncilMeetingDialogue = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Text = "As you enter the study, you're surprised to find it packed with people. Senator Quintus and High Priestess Caelia stand at the head of the table, ancient tomes and the cultist documents spread before them. Your guild members - all of them - are crowded around, their expressions grave.<br><br>Quintus looks up as you enter. \"Thank you for coming. We've gathered everyone because what Caelia and I discovered affects us all.\" His voice carries unusual weight. \"This cult - they call themselves the Unbound - they're not just planning an assassination. They're planning to break the Five Seals.\"",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"The Five Seals? What are those?\"",
                            nextNodeID = "explain_seals"
                        }
                    }
                },
                ["explain_seals"] = new DialogueNode
                {
                    Text = "Caelia's silver eyes find yours. \"In the age before the Empire, five heroes bound the primordial entities that fed on chaos and fear. The Seals aren't physical objects - they're anchors of order, maintained through ritual and collective belief.\"<br><br>Quintus taps the documents. \"The cult plans to assassinate Emperor Certius during the 1500th anniversary celebration in Aevoria. Thousands of witnesses, maximum chaos and terror - they'll use that energy to crack the first seal.\"<br><br>One of your recruits speaks up, voice shaking. \"What happens if a seal breaks?\"",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"Tell us the truth. How bad is it?\"",
                            nextNodeID = "seal_breaks"
                        }
                    }
                },
                ["seal_breaks"] = new DialogueNode
                {
                    Text = "Caelia's expression is grim. \"The seals are interconnected. If one fails, the cascade unravels all five within days. The entities that were bound...\" She pauses, choosing her words carefully. \"Madness. Entropy. The dissolution of natural law itself. Everything ends.\"<br><br>The room falls silent. Quintus breaks it. \"This is why we must act now. The festival is in five days. We have one chance to stop this.\"",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"Then I'll go to Aevoria and warn the Emperor.\"",
                            nextNodeID = "decision_made"
                        }
                    }
                },
                ["decision_made"] = new DialogueNode
                {
                    Text = "Quintus nods with satisfaction. \"I knew we could count on you. Caelia and I will accompany you - this requires my senatorial authority and her expertise on the Seals.\"<br><br>Quintus stands, gathering the documents. \"The journey to Aevoria takes three days by the Imperial Highway. We leave immediately. Your guild members will hold the fort here while we're gone.\"<br><br>You look around at the faces of your recruits - determination mixed with fear. They know what's at stake.",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"Let's not waste any time. We ride now.\"",
                            nextNodeID = "begin_journey"
                        }
                    }
                },
                ["begin_journey"] = new DialogueNode
                {
                    Text = "The journey north is tense but uneventful. The Imperial Highway is well-maintained, cutting through rolling farmland and ancient forests. Caelia and Quintus spend much of the ride discussing the Seals and the cult's probable tactics.<br><br>On the third day, you crest a hill and see it:<br><br>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━<br>           AEVORIA, THE ETERNAL CITY<br>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━<br><br>White marble buildings gleam in the sunlight, their grandeur seeming to touch the heavens. The massive Imperial Palace dominates the city's heart, its golden dome blazing like a second sun. To the east rises the legendary Colosseum, where emperors have celebrated triumphs for fifteen hundred years.<br><br>As you approach the western gate, Quintus presents his senatorial seal. The guards - resplendent in crimson cloaks and polished armor - snap to attention and wave you through without question.<br><br>\"The Emperor's villa is this way,\" Quintus says, leading you through pristine streets lined with marble statues and flowering gardens. \"I sent word ahead. Certius will see us. The celebration begins in two days - we have time to prepare.\"",
                    Action = new DialogueAction
                    {
                        Type = "force_travel",
                        Parameters = { { "room_id", "200" }, { "silent", "true" } }
                    },
                    Choices = new List<DialogueNode.Choice>() // No choices - dialogue ends and travels to Aevoria
                }
            };
            dialogueManager.RegisterEventDialogue("guild_council_meeting_dialogue", guildCouncilMeetingDialogue);

            // Register celebration start dialogue
            var celebrationStartDialogue = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Text = "You awaken in your guest quarters to urgent knocking. A servant enters, bowing quickly. \"The celebration begins within the hour! Senator Quintus and Lady Caelia request your presence in the grand hall immediately.\"<br><br>You dress quickly and descend to find Quintus and Caelia already waiting, fully armored. The lighthearted mood from two days ago is gone - their expressions are grim.<br><br>\"The time has come,\" Quintus says quietly. \"The Colosseum is filling with spectators. Thousands of people. If the cult makes their move...\" He doesn't finish the thought.",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"Then we stop them. Let's go.\"",
                            nextNodeID = "depart_for_colosseum"
                        }
                    }
                },
                ["depart_for_colosseum"] = new DialogueNode
                {
                    Text = "Caelia places a hand on your shoulder. \"May Keius protect us all today.\" There's real fear in her eyes - the first time you've seen her composure crack.<br><br>Your party assembles in full battle gear. As you leave the villa and make your way through the crowded streets toward the Colosseum, the festive atmosphere feels surreal. Citizens in their finest clothes celebrate fifteen centuries of empire, unaware of the apocalyptic threat looming.<br><br>The Colosseum rises before you, its massive arches packed with spectators. The roar of the crowd is deafening. Somewhere in this chaos, cultists are waiting to strike.<br><br>Quintus leads you through a guarded entrance. \"The Emperor's seat box is on the north side. We'll need to fight through the outer galleries to reach him. Stay alert - anyone could be a cultist.\"",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"I'm ready. Let's move.\"",
                            nextNodeID = "enter_colosseum"
                        }
                    }
                },
                ["enter_colosseum"] = new DialogueNode
                {
                    Text = "You step into the lower gallery of the Colosseum. The noise of the crowd above is muffled here, replaced by an eerie silence. Then you see them - two figures in dark robes blocking the passage ahead. When they notice you, they reach for their weapons.<br><br>\"Cultists!\" Quintus shouts. \"They're already here!\"",
                    Action = new DialogueAction
                    {
                        Type = "force_travel",
                        Parameters = { { "room_id", "220" } }
                    },
                    Choices = new List<DialogueNode.Choice>()
                }
            };
            dialogueManager.RegisterEventDialogue("celebration_start_dialogue", celebrationStartDialogue);

            // Register assassination scene dialogue
            var assassinationDialogue = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Text = "You burst into the Emperor's seat box and freeze. The scene before you unfolds in slow motion.<br><br>Emperor Certius stands at the railing, waving to the roaring crowd below. Behind him, a figure in imperial guard armor moves with inhuman speed. You shout a warning, but it's lost in the noise of the crowd.<br><br>The blade enters between the Emperor's ribs. Certius's eyes go wide with shock. The assassin - face concealed by their helmet - withdraws the dagger and melts back into the crowd of guards before you can react.",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"NO! Guards, stop that assassin!\"",
                            nextNodeID = "guards_confused"
                        }
                    }
                },
                ["guards_confused"] = new DialogueNode
                {
                    Text = "The imperial guards spin in confusion - which one? They all look identical in their ceremonial armor. The assassin has already disappeared through a service entrance.<br><br>You rush to the Emperor's side as he collapses. Quintus and Caelia are right behind you. Blood spreads across Certius's purple robes. His breath comes in shallow gasps.",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "Kneel beside the Emperor",
                            nextNodeID = "emperor_dying"
                        }
                    }
                },
                ["emperor_dying"] = new DialogueNode
                {
                    Text = "You kneel, and Certius's eyes find yours. Despite the pain, there's a flicker of recognition. He tries to speak, blood flecking his lips. \"You... were right... I'm sorry...\"<br><br>His hand grips yours with surprising strength. \"Tell... Livia... I love her... Tell my children...\" His voice fades. \"Protect... the Empire...\"<br><br>The light fades from Emperor Certius Rex Maximus's eyes. The man who built an empire, who dreamed of academies and exploration, who woke each morning to watch the sunrise - dead on the stone floor of his beloved Colosseum.",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"I will. I swear it.\"",
                            nextNodeID = "assassin_fight"
                        }
                    }
                },
                ["assassin_fight"] = new DialogueNode
                {
                    Text = "A shadow falls across you. You look up to see the assassin has returned - helmet removed, revealing a cultist's tattooed face twisted in zealous fury.<br><br>\"The first seal cracks even now!\" the assassin screams. \"Ten thousand witnesses! Maximum chaos! The Unbound thank you for gathering them all in one place!\"<br><br>The cultist raises their bloodied dagger and lunges at you.",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "Draw your weapon!",
                            nextNodeID = "begin_combat"
                        }
                    }
                },
                ["begin_combat"] = new DialogueNode
                {
                    Text = "You rise to meet the assassin's charge, weapon in hand. This ends now.",
                    Action = new DialogueAction { Type = "trigger_combat" },
                    Choices = new List<DialogueNode.Choice>()
                }
            };
            dialogueManager.RegisterEventDialogue("assassination_scene", assassinationDialogue);

            // Register aftermath dialogue
            var aftermathDialogue = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Text = "The assassin's body lies still on the marble floor, joining the Emperor's in death. The weight of what just happened settles over you like a shroud.<br><br>Quintus and Caelia rush into the box, their faces stricken with horror as they see Certius's lifeless form. Caelia falls to her knees beside the Emperor, placing a gentle hand on his forehead. Tears stream down her face.<br><br>\"He's gone,\" she whispers. \"The first seal... I can feel it. A crack, but not broken. Not yet.\" Her voice strengthens. \"But if more pillars fall...\"<br><br>Quintus grips the railing, knuckles white. \"The whole Empire will descend into chaos when word spreads. Exactly what they wanted.\"",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"What happens now?\"",
                            nextNodeID = "whats_next"
                        }
                    }
                },
                ["whats_next"] = new DialogueNode
                {
                    Text = "Quintus takes a shaky breath, forcing himself into senator mode. \"The Senate will convene within hours. The generals will demand action. They'll choose a new emperor quickly - probably Tribune Marcellus. Young, popular with the legions, politically acceptable.\"<br><br>He looks directly at you. \"And they'll demand a response to this attack. The official story will be that we're hunting the cult with every resource available. Special inquisitions, expanded guard patrols, public trials.\"<br><br>His expression darkens. \"But we know better. Many of those same senators and generals are probably Unbound themselves. It's the perfect cover - appear to hunt cultists while actually protecting them.\"",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"Then what can we do?\"",
                            nextNodeID = "what_we_do"
                        }
                    }
                },
                ["what_we_do"] = new DialogueNode
                {
                    Text = "Caelia stands, composing herself. \"We continue the fight. Quietly. The Unbound will move more openly now - they've shown their hand. Other 'pillars' will be targeted. Other seals weakened.\"<br><br>Quintus nods grimly. \"Your guild is our best asset. Official channels are compromised, but your people answer only to you. I'll feed you intelligence from my position in the Senate. Caelia will provide spiritual guidance and seal knowledge.\"<br><br>He places a hand on your shoulder. \"We couldn't save Certius. But we can honor his last words - protect the Empire. Stop the other seals from breaking. It's all we have left.\"",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"I understand. What's our first move?\"",
                            nextNodeID = "first_move"
                        }
                    }
                },
                ["first_move"] = new DialogueNode
                {
                    Text = "\"Return to Belum,\" Quintus says. \"Gather your strength. I'll stay here to manage the political fallout and gather information. When I learn which pillar they're targeting next, I'll send word through secured channels.\"<br><br>Caelia adds quietly, \"And I must return to my temple. The seals must be monitored constantly now. Every tremor, every crack - I'll feel it and warn you.\"<br><br>You take one last look at Emperor Certius. A good man. A great emperor. Dead because you arrived too late.<br><br>Quintus reads your expression. \"Don't. You tried to warn him. He chose not to listen. All we can do now is make sure his death wasn't in vain.\"",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"For Certius. For the Empire.\"",
                            nextNodeID = "return_to_guild"
                        }
                    }
                },
                ["return_to_guild"] = new DialogueNode
                {
                    Text = "The journey back to Belum passes in a blur of grief and determination. The roads are already buzzing with news of the Emperor's death. Citizens weep openly. Soldiers march with grim purpose. The Empire mourns.<br><br>Your party travels in silence, each lost in their own thoughts. When you finally see the familiar walls of Belum rising before you, there's a sense of both relief and dread. The real fight is only beginning.<br><br>You return to your guild hall, weary but unbroken. The fight against the Unbound continues.",
                    Action = new DialogueAction
                    {
                        Type = "force_travel",
                        Parameters = { { "room_id", "3" } }  // Guild Hall Study
                    },
                    Choices = new List<DialogueNode.Choice>()
                }
            };
            dialogueManager.RegisterEventDialogue("aftermath_dialogue", aftermathDialogue);

            // Register Act II intro dialogue
            var actTwoIntroDialogue = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Text = "You find Senator Quintus waiting in your study, a glass of wine in hand. He looks older than when you first met - the events at Aevoria have aged him. He raises the glass in grim salute as you enter.<br><br>\"Welcome back, guild master. I hope you've rested - because the real fight is only beginning.\" He sets down the glass and unfurls a map across your desk. \"I've been gathering intelligence from my contacts in the Senate and military. The Unbound are moving more boldly now, emboldened by their 'success' at Aevoria.\"",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"What have you learned?\"",
                            nextNodeID = "intelligence_report"
                        }
                    }
                },
                ["intelligence_report"] = new DialogueNode
                {
                    Text = "Quintus points to various locations on the map. \"Cult activity has been reported across the Empire. Temple desecrations. Mysterious disappearances. Strange rituals. Each incident weakens the fabric of order that holds the seals in place.\"<br><br>He taps the map. \"I can't investigate all of these personally - I'm being watched too closely by cultists within the Senate. But your guild...\" He looks up at you. \"Your people are perfect for this. Unknown faces. No official ties. They can go where I cannot.\"",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"You want me to send my recruits on missions?\"",
                            nextNodeID = "guild_quests_explained"
                        }
                    }
                },
                ["guild_quests_explained"] = new DialogueNode
                {
                    Text = "\"Exactly.\" Quintus pulls out a leather-bound ledger. \"I've compiled reports from across the Empire - situations that need investigation, threats that need neutralizing, people who need protecting. Too many for you to handle alone, but perfect for a well-managed guild.\"<br><br>He hands you the ledger. \"Send your recruits out as you see fit. They'll gain experience, earn rewards, and most importantly - gather intelligence on the Unbound's plans. When you access your guild management, you'll find these assignments waiting. Review them carefully and assign the right people for each task.\"",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "\"This is a smart approach. My people are ready.\"",
                            nextNodeID = "commitment"
                        }
                    }
                },
                ["commitment"] = new DialogueNode
                {
                    Text = "Quintus nods with satisfaction. \"I knew I could count on you. Caelia is monitoring the seals from her temple, I'm watching the political situation, and you...\" He places a hand on your shoulder. \"You're building the force that will actually stop them.\"<br><br>\"The ledger will update as new threats emerge. Check it regularly through your guild management menu. May the gods guide your decisions, guild master. The Empire's survival depends on them.\"<br><br>He gathers his things to leave, pausing at the door. \"And rest when you can. The darkness ahead is long.\"",
                    Action = new DialogueAction
                    {
                        Type = "show_tutorial",
                        Parameters = { { "tutorial_id", "guild_quest_ledger" } }
                    },
                    Choices = new List<DialogueNode.Choice>()
                }
            };
            dialogueManager.RegisterEventDialogue("act_two_intro_dialogue", actTwoIntroDialogue);

            // Register colosseum cinematic dialogues
            var colosseumLowerGalleryDialogue = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Text = "The roar of the crowd echoes through the stone corridor. Ahead, two figures in cultist robes turn to face you. One of them draws a curved blade, grinning with zealous confidence.<br><br>\"The Unbound will break the seals!\" the cultist shouts. \"You cannot stop what has already begun!\"",
                    Choices = new List<DialogueNode.Choice>()
                }
            };
            dialogueManager.RegisterEventDialogue("colosseum_lower_gallery_dialogue", colosseumLowerGalleryDialogue);

            var colosseumMidGalleryDialogue = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Text = "You climb the stairs to the mid-level gallery. More cultists stand ready, their armor marked with the symbol of entropy - a circle breaking apart.<br><br>A cultist zealot steps forward, weapon raised. \"The Emperor dies today. The first seal cracks. Nothing you do matters anymore!\"<br><br>Caelia's voice is urgent behind you. \"They're stalling us! We have to reach the Emperor now!\"",
                    Choices = new List<DialogueNode.Choice>()
                }
            };
            dialogueManager.RegisterEventDialogue("colosseum_mid_gallery_dialogue", colosseumMidGalleryDialogue);

            var colosseumUpperGalleryDialogue = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Text = "The upper gallery opens before you. Four cultists - clearly veterans - stand in formation, blocking the final passage to the Emperor's box. Their leader, a scarred lieutenant, raises a hand to halt you.<br><br>\"The seal cracks as we speak,\" the lieutenant says coldly. \"You're too late to stop it. But you're welcome to die trying.\"<br><br>From somewhere above, you hear a terrible scream cut short. Quintus's face goes pale. \"That came from the Emperor's box. We're out of time!\"",
                    Choices = new List<DialogueNode.Choice>()
                }
            };
            dialogueManager.RegisterEventDialogue("colosseum_upper_gallery_dialogue", colosseumUpperGalleryDialogue);
        }

        /// <summary>
        /// Farm bandit attack event - first visit to Gaius' farm
        /// </summary>
        private static EventData CreateFarmBanditAttackEvent()
        {
            return new EventData("farm_bandit_attack", 10)  // Room 10 is Gaius' Farm Fields
            {
                Priority = 50,
                IsOneTime = true,
                DialogueTreeId = "",  // No dialogue tree, just a message
                Conditions = new List<EventCondition>
                {
                    // Triggers only on first visit to the farm
                    new EventCondition(ConditionType.FirstVisit, "farm_bandit_attack", true)
                },
                Actions = new List<EventAction>
                {
                    // Show message about the bandit attack
                    new EventAction(ActionType.DisplayMessage)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "message", "\n[bold yellow]As you approach the farm fields, you see signs of a recent struggle.[/]\n\nBroken fences, trampled crops, and the unmistakable marks of combat. It appears bandits have begun attacking Gaius' farm while he was away at the crossroads. You hear angry voices ahead - the bandits are still here!\n" }
                        }
                    },
                    // Set quest flag
                    new EventAction(ActionType.SetQuestFlag)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "flagId", "farm_bandits_encountered" },
                            { "value", true }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Brief cinematic when entering Colosseum Lower Gallery during the celebration
        /// </summary>
        private static EventData CreateColosseumLowerGalleryEvent()
        {
            return new EventData("colosseum_lower_gallery_cinematic", 220)
            {
                Priority = 100,
                IsOneTime = true,
                DialogueTreeId = "colosseum_lower_gallery_dialogue",
                Conditions = new List<EventCondition>
                {
                    new EventCondition(ConditionType.FirstVisit, "colosseum_lower_gallery_cinematic", true)
                },
                Actions = new List<EventAction>()
            };
        }

        /// <summary>
        /// Brief cinematic when entering Colosseum Mid Gallery during the celebration
        /// </summary>
        private static EventData CreateColosseumMidGalleryEvent()
        {
            return new EventData("colosseum_mid_gallery_cinematic", 221)
            {
                Priority = 100,
                IsOneTime = true,
                DialogueTreeId = "colosseum_mid_gallery_dialogue",
                Conditions = new List<EventCondition>
                {
                    new EventCondition(ConditionType.FirstVisit, "colosseum_mid_gallery_cinematic", true)
                },
                Actions = new List<EventAction>()
            };
        }

        /// <summary>
        /// Brief cinematic when entering Colosseum Upper Gallery during the celebration
        /// </summary>
        private static EventData CreateColosseumUpperGalleryEvent()
        {
            return new EventData("colosseum_upper_gallery_cinematic", 222)
            {
                Priority = 100,
                IsOneTime = true,
                DialogueTreeId = "colosseum_upper_gallery_dialogue",
                Conditions = new List<EventCondition>
                {
                    new EventCondition(ConditionType.FirstVisit, "colosseum_upper_gallery_cinematic", true)
                },
                Actions = new List<EventAction>()
            };
        }
    }
}
