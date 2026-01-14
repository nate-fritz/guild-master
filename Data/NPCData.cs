using GuildMaster.Managers;
using GuildMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GuildMaster.Data
{
    public class NPCData
    {
        public static Dictionary<string, NPC> InitializeNPCs()
        {
            var npcs = new Dictionary<string, NPC>();
       
            NPC townGuard = new NPC();
            townGuard.Name = "Town Guard";
            townGuard.Description = "A guard in bronze armor keeps watch over the area.";
            townGuard.ShortDescription = "A town guard";
            townGuard.IsHostile = false;
            townGuard.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "The guard glances at you briefly and says \"Move along, citizen. Keep your nose clean and we won't have any problems.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Can you give me directions?\"", nextNodeID = "ask_directions" },
                    new DialogueNode.Choice { choiceText = "\"Understood.\"", nextNodeID = "end" }
                }
            });

            townGuard.Dialogue.Add("ask_directions", new DialogueNode()
            {
                Text = "The guard nods. \"What are you looking for?\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Where is the Town Hall?\"", nextNodeID = "dir_town_hall" },
                    new DialogueNode.Choice { choiceText = "\"Where is the Temple District?\"", nextNodeID = "dir_temple" },
                    new DialogueNode.Choice { choiceText = "\"Where can I find the tavern?\"", nextNodeID = "dir_tavern" },
                    new DialogueNode.Choice { choiceText = "\"Where's the blacksmith?\"", nextNodeID = "dir_blacksmith" },
                    new DialogueNode.Choice { choiceText = "\"Never mind.\"", nextNodeID = "end" }
                }
            });

            townGuard.Dialogue.Add("dir_town_hall", new DialogueNode()
            {
                Text = "\"The Town Hall is in the western part of Belum. Head to the town square and go west through the residential area. You can't miss it - big marble columns and bronze doors.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Anything else?\"", nextNodeID = "ask_directions" },
                    new DialogueNode.Choice { choiceText = "\"Thank you.\"", nextNodeID = "end" }
                }
            });

            townGuard.Dialogue.Add("dir_temple", new DialogueNode()
            {
                Text = "\"The Temple District is northwest of the town square. Head north to the residential area, then west. The temples surround a central plaza.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Anything else?\"", nextNodeID = "ask_directions" },
                    new DialogueNode.Choice { choiceText = "\"Thank you.\"", nextNodeID = "end" }
                }
            });

            townGuard.Dialogue.Add("dir_tavern", new DialogueNode()
            {
                Text = "\"The Golden Grape? Popular place. It's in the eastern part of town. From the town square, head east and you'll find it in the poor quarter area. Can't miss the golden grape sign.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Anything else?\"", nextNodeID = "ask_directions" },
                    new DialogueNode.Choice { choiceText = "\"Thank you.\"", nextNodeID = "end" }
                }
            });

            townGuard.Dialogue.Add("dir_blacksmith", new DialogueNode()
            {
                Text = "\"The Iron Anvil is in the southern market district. From the south gate, head north through the market and you'll hear the forges. Big place, can't miss it.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Anything else?\"", nextNodeID = "ask_directions" },
                    new DialogueNode.Choice { choiceText = "\"Thank you.\"", nextNodeID = "end" }
                }
            });

            townGuard.Dialogue.Add("end", new DialogueNode()
            {
                Text = "The guard nods and returns to his duties.",
                Choices = { }
            });

            npcs.Add(townGuard.Name, townGuard);

            NPC villager = new NPC();
            villager.Name = "Villager";
            villager.Description = "A local going about their daily business.";
            villager.ShortDescription = "A villager";
            villager.IsHostile = false;
            villager.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "The villager greets you with a friendly smile. \"Good day to you, traveler. Welcome to Belum!\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Can you help me find my way around?\"", nextNodeID = "ask_directions" },
                    new DialogueNode.Choice { choiceText = "\"Good day to you as well.\"", nextNodeID = "end" }
                }
            });

            villager.Dialogue.Add("ask_directions", new DialogueNode()
            {
                Text = "\"Of course! I'd be happy to help. What are you looking for?\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Where is the Town Hall?\"", nextNodeID = "dir_town_hall" },
                    new DialogueNode.Choice { choiceText = "\"Where is the Temple District?\"", nextNodeID = "dir_temple" },
                    new DialogueNode.Choice { choiceText = "\"Where can I get a drink?\"", nextNodeID = "dir_tavern" },
                    new DialogueNode.Choice { choiceText = "\"Where's the blacksmith?\"", nextNodeID = "dir_blacksmith" },
                    new DialogueNode.Choice { choiceText = "\"I'm all set, thank you.\"", nextNodeID = "end" }
                }
            });

            villager.Dialogue.Add("dir_town_hall", new DialogueNode()
            {
                Text = "\"The Town Hall? That's on the western side of town. Go to the town square and head west through the nice residential area. You'll see it - big impressive building with marble columns!\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"What else is there?\"", nextNodeID = "ask_directions" },
                    new DialogueNode.Choice { choiceText = "\"Thank you!\"", nextNodeID = "end" }
                }
            });

            villager.Dialogue.Add("dir_temple", new DialogueNode()
            {
                Text = "\"Ah, the temples! Northwest of the town square. Head north from the square to the residential area, then west. There's a whole plaza surrounded by temples to the different gods.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"What else is there?\"", nextNodeID = "ask_directions" },
                    new DialogueNode.Choice { choiceText = "\"Thank you!\"", nextNodeID = "end" }
                }
            });

            villager.Dialogue.Add("dir_tavern", new DialogueNode()
            {
                Text = "\"The Golden Grape is the best tavern in Belum! It's in the eastern part of town. From the square, head east and you'll find it. Look for the golden grape on the sign - can't miss it!\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"What else is there?\"", nextNodeID = "ask_directions" },
                    new DialogueNode.Choice { choiceText = "\"Thank you!\"", nextNodeID = "end" }
                }
            });

            villager.Dialogue.Add("dir_blacksmith", new DialogueNode()
            {
                Text = "\"The Iron Anvil is the best smithy around! It's in the southern part of town near the market. If you're coming from the south gate, head north through the market district and follow your ears - you'll hear the hammers on the anvils!\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"What else is there?\"", nextNodeID = "ask_directions" },
                    new DialogueNode.Choice { choiceText = "\"Thank you!\"", nextNodeID = "end" }
                }
            });

            villager.Dialogue.Add("end", new DialogueNode()
            {
                Text = "The villager waves cheerfully and continues on their way.",
                Choices = { }
            });

            npcs.Add(villager.Name, villager);

            NPC merchant = new NPC();
            merchant.Name = "Merchant";
            merchant.Description = "A trader with goods from distant lands.";
            merchant.ShortDescription = "A merchant";
            merchant.IsHostile = false;
            merchant.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "The merchant spreads his arms wide and says \"Looking to buy? Looking to sell? Either way, you've come to the right place!\"",
                Choices = { }
            });
            npcs.Add(merchant.Name, merchant);

            NPC barkeep = new NPC();
            barkeep.Name = "Barkeep";
            barkeep.Description = "A jovial man with a towel over his shoulder wipes down the bar.";
            barkeep.ShortDescription = "The barkeep";
            barkeep.IsHostile = false;
            barkeep.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "The barkeep looks up from wiping down the bar and greets you warmly. \"Welcome to the Golden Grape! What'll it be? Ale? Wine? Or perhaps you're looking for information?\"",
                Choices = { }
            });
            npcs.Add(barkeep.Name, barkeep);

            NPC blacksmith = new NPC();
            blacksmith.Name = "Blacksmith";
            blacksmith.Description = "A massive man with bulging arms works the forge, sweat glistening on his brow.";
            blacksmith.ShortDescription = "The blacksmith";
            blacksmith.IsHostile = false;
            blacksmith.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "The blacksmith sets down his hammer and wipes the sweat from his brow. \"Need something forged? Repaired? I'm the best smith in Belum - ask anyone.\"",
                Choices = { }
            });

            // Set up as vendor
            blacksmith.IsVendor = true;
            blacksmith.BuybackMultiplier = 0.5f; // Buys items at 50% value

            // Shop inventory (item name -> price)
            blacksmith.ShopInventory.Add("iron gladius", 50);
            blacksmith.ShopInventory.Add("battle axe", 60);
            blacksmith.ShopInventory.Add("hunter's bow", 55);
            blacksmith.ShopInventory.Add("steel gladius", 120);
            blacksmith.ShopInventory.Add("leather armor", 40);
            blacksmith.ShopInventory.Add("chainmail", 80);
            blacksmith.ShopInventory.Add("iron helm", 35);

            npcs.Add(blacksmith.Name, blacksmith);

            // Apothecary - Sells potions and remedies
            NPC apothecary = new NPC();
            apothecary.Name = "Apothecary";
            apothecary.Description = "A thin, scholarly woman with ink-stained fingers carefully organizes vials and jars. The scent of herbs and alchemical reagents fills the air around her shop.";
            apothecary.ShortDescription = "The apothecary";
            apothecary.IsHostile = false;
            apothecary.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "The apothecary looks up from her work and smiles. \"Welcome to my apothecary. I stock the finest potions and remedies in all of Belum. What can I prepare for you today?\"",
                Choices = { }
            });

            // Set up as vendor
            apothecary.IsVendor = true;
            apothecary.BuybackMultiplier = 0.4f; // Buys items at 40% value (potions are more specialized)

            // Shop inventory (item name -> price)
            apothecary.ShopInventory.Add("potion", 25);
            apothecary.ShopInventory.Add("energy potion", 30);
            apothecary.ShopInventory.Add("greater potion", 60);
            apothecary.ShopInventory.Add("greater energy potion", 70);
            apothecary.ShopInventory.Add("elixir of vigor", 100);
            apothecary.ShopInventory.Add("antidote", 20);

            npcs.Add(apothecary.Name, apothecary);

            // Scribe - Sells scrolls and written goods
            NPC scribe = new NPC();
            scribe.Name = "Scribe";
            scribe.Description = "An elderly man with careful hands and keen eyes sits among stacks of scrolls and parchments. His workspace is meticulously organized, with quills and inks arranged by color.";
            scribe.ShortDescription = "The scribe";
            scribe.IsHostile = false;
            scribe.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "The scribe carefully sets down his quill and regards you with keen eyes. \"Greetings, traveler. I deal in scrolls, both mundane and mystical. Are you in need of knowledge... or power?\"",
                Choices = { }
            });

            // Set up as vendor
            scribe.IsVendor = true;
            scribe.BuybackMultiplier = 0.3f; // Buys items at 30% value (scrolls are one-use, specialized)

            // Shop inventory (item name -> price)
            scribe.ShopInventory.Add("restoration scroll", 50);
            scribe.ShopInventory.Add("scroll of fireball", 80);
            scribe.ShopInventory.Add("scroll of healing", 70);
            scribe.ShopInventory.Add("scroll of protection", 90);
            scribe.ShopInventory.Add("scroll of haste", 100);
            scribe.ShopInventory.Add("teleport scroll", 150);

            npcs.Add(scribe.Name, scribe);

            // Guild Armorer - Sells mid-tier equipment in guild armory (Room 65)
            NPC guildArmorer = new NPC();
            guildArmorer.Name = "Guild Armorer";
            guildArmorer.Description = "A grizzled veteran with a master smith's apron stands before an array of exceptional weapons and armor. Scars and burn marks on his weathered hands speak to decades of forge work. His eyes gleam with professional pride as he examines each piece of equipment.";
            guildArmorer.ShortDescription = "A master armorer";
            guildArmorer.IsHostile = false;
            guildArmorer.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "The armorer nods respectfully as you approach. \"Welcome to the guild armory. Your reputation precedes you - only the finest equipment for proven adventurers. What catches your eye?\"",
                Choices = { }
            });

            // Set up as vendor
            guildArmorer.IsVendor = true;
            guildArmorer.BuybackMultiplier = 0.6f; // Better buyback than town shops

            // Shop inventory - Mid-tier equipment
            guildArmorer.ShopInventory.Add("mithril sword", 200);
            guildArmorer.ShopInventory.Add("elven bow", 180);
            guildArmorer.ShopInventory.Add("arcane staff", 160);
            guildArmorer.ShopInventory.Add("war hammer", 220);
            guildArmorer.ShopInventory.Add("dragon scale armor", 300);
            guildArmorer.ShopInventory.Add("mithril chainmail", 250);
            guildArmorer.ShopInventory.Add("plate armor", 100);
            guildArmorer.ShopInventory.Add("crown of focus", 75);

            npcs.Add(guildArmorer.Name, guildArmorer);

            // Gate Guard - Quest giver for bandit cave quest
            NPC sentry = new NPC();
            sentry.Name = "Marcus";
            sentry.Description = "A weathered veteran guard in bronze armor stands at attention. His scarred face and alert eyes suggest many years of service. The crest of Belum is emblazoned on his breastplate.";
            sentry.ShortDescription = "Belum South Gate Sentry";
            sentry.IsHostile = false;

            // Initial dialogue - explains closed gate
            sentry.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "As you approach the gate, the sentry adjusts his shield and spear, then barks \"Halt, traveler. This gate to Belum is closed by order of the town council. None may enter.\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"I have the Bandit Warlord's head.\" (Show him the head)",
                        nextNodeID = "quest_complete",
                        IsAvailable = (inventory) => inventory.Contains("warlord's head"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "warlord's head"} } }
                    },
                    new DialogueNode.Choice { choiceText = "\"Why is the gate closed?\"", nextNodeID = "explain_bandits" },
                    new DialogueNode.Choice { choiceText = "\"I understand. I'll move along.\"", nextNodeID = "end" }
                }
            });

            sentry.Dialogue.Add("explain_bandits", new DialogueNode()
            {
                Text = "Marcus scowls. \"Bandits. A large group has made camp in caves to the southwest - west of Gaius' farm, south of the mountains. They've been raiding travelers and farms. Until they're dealt with, the council won't allow the southern gate to open. Too dangerous.\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"You're not going to believe this, but I have the Warlord's head.\" (Show him)",
                        nextNodeID = "quest_complete",
                        IsAvailable = (inventory) => inventory.Contains("warlord's head"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "warlord's head"} } }
                    },
                    new DialogueNode.Choice { choiceText = "\"I could deal with these bandits.\"", nextNodeID = "offer_help" },
                    new DialogueNode.Choice { choiceText = "\"That sounds dangerous. I'll stay away.\"", nextNodeID = "end" }
                }
            });

            sentry.Dialogue.Add("offer_help", new DialogueNode()
            {
                Text = "He sizes you up. \"You?\" He pauses, then continues, \"...Perhaps. The leader calls himself the Bandit Warlord. Pompous bastard. If you can kill him and bring me proof - his head will do - I'll convince the council to open the gate.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I'll do it. The Warlord will fall.\"", nextNodeID = "quest_accepted" },
                    new DialogueNode.Choice { choiceText = "\"Let me think about it.\"", nextNodeID = "end" }
                }
            });

            sentry.Dialogue.Add("quest_accepted", new DialogueNode()
            {
                Text = "Marcus nods grimly. \"Good luck. You'll need it. Be careful down there - the bandits fight dirty and they outnumber you. Return with the Warlord's head and your reward will be access to Belum.\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"I have the Bandit Warlord's head.\" (Show him)",
                        nextNodeID = "quest_complete",
                        IsAvailable = (inventory) => inventory.Contains("warlord's head"),
                        Action = new DialogueAction {
                            Type = "give_item",
                            Parameters = { { "item", "warlord's head" } }
                        }
                    },
                    new DialogueNode.Choice {
                        choiceText = "\"I'm still working on it.\"",
                        nextNodeID = "end"
                    }
                }
            });

            // Quest completion dialogue - player has the warlord's head
            sentry.Dialogue.Add("quest_complete", new DialogueNode()
            {
                Text = "Marcus' eyes widen as you unwrap the grisly trophy. \"By the gods... you actually did it! The Warlord's head!\" He examines it grimly. \"This will send a message to any other bandits thinking of setting up shop here. Well done, adventurer. I'll inform the council immediately - the southern gate is now open to you.\"<br><br>He pauses, then adds, \"If you found anything unusual on the Warlord - letters, documents, anything like that - you should take them to Senator Quintus in the town hall. He's skilled with ciphers and codes. Might be able to make sense of it.\"",
                Action = new DialogueAction { Type = "open_gate" },
                Choices = { }
            });

            // After quest is complete
            sentry.Dialogue.Add("after_quest", new DialogueNode()
            {
                Text = "Marcus nods at you respectfully. \"Thank you again for taking care of those bandits. The roads are safer now because of you.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Just doing my part.\"", nextNodeID = "end" },
                    new DialogueNode.Choice { choiceText = "\"Happy to help. Take care, Marcus.\"", nextNodeID = "end" }
                }
            });

            sentry.Dialogue.Add("end", new DialogueNode()
            {
                Text = "He nods curtly. \"Stay safe out there.\"",
                Choices = { }
            });

            npcs.Add(sentry.Name, sentry);

            NPC priestess = new NPC();
            priestess.Name = "Caelia";
            priestess.Description = "A woman of striking beauty stands before the altar, her age impossible to determine - she could be anywhere from thirty to three hundred. Golden hair cascades down her back, and her silver eyes hold an otherworldly luminescence. She wears the ornate robes of a High Priestess of Keius, marked with symbols of light and knowledge. As you approach, she turns with a knowing smile, as if she sensed your presence before you entered.";
            priestess.ShortDescription = "High Priestess Caelia";

            // First greeting
            priestess.Dialogue.Add("first_greeting", new DialogueNode()
            {
                Text = "The priestess regards you with eyes that seem to see more than most. \"Welcome to the Temple of Keius, traveler. I am Caelia, High Priestess of this sanctuary.\" Her voice is melodious, almost musical.<br><br>She tilts her head slightly, studying you. \"You carry yourself like one who has seen battle. A guild master, perhaps?\" A knowing smile touches her lips. \"My old friend Quintus mentioned that the guild might be reformed. Come seeking guidance, or merely respite?\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"Quintus said I could trust you. I found a translated letter with strange words - 'Ordo Dissolutus'. What does it mean?\"",
                        nextNodeID = "ask_about_passphrase",
                        IsAvailable = (inventory) => inventory.Contains("translated letter"),
                        RequireNotDiscussedNode = "ask_about_passphrase"
                    },
                    new DialogueNode.Choice { choiceText = "\"Just looking around. Beautiful temple.\"", nextNodeID = "end" },
                    new DialogueNode.Choice { choiceText = "\"Tell me about Keius.\"", nextNodeID = "about_keius" }
                }
            });

            // Repeat greeting
            priestess.Dialogue.Add("repeat_greeting", new DialogueNode()
            {
                Text = "Caelia's luminous eyes find yours as you approach. \"Welcome back, guild master.\" There's a hint of warmth in her smile. \"Quintus speaks highly of you. How may I assist you today?\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"Quintus said I could trust you. I found a translated letter with strange words - 'Ordo Dissolutus'. What does it mean?\"",
                        nextNodeID = "ask_about_passphrase",
                        IsAvailable = (inventory) => inventory.Contains("translated letter"),
                        RequireNotDiscussedNode = "ask_about_passphrase"
                    },
                    new DialogueNode.Choice { choiceText = "\"Tell me about Keius.\"", nextNodeID = "about_keius" },
                    new DialogueNode.Choice { choiceText = "\"Just passing through.\"", nextNodeID = "end" }
                }
            });

            priestess.Dialogue.Add("about_keius", new DialogueNode()
            {
                Text = "Caelia's expression becomes reverent. \"Keius is the god of light, knowledge, and truth. He teaches that understanding dispels darkness, and that wisdom is the greatest weapon against chaos.\" She gestures to the temple around you. \"This sanctuary has stood for centuries, a beacon against ignorance and fear.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Thank you for explaining.\"", nextNodeID = "end" }
                }
            });

            priestess.Dialogue.Add("ask_about_passphrase", new DialogueNode()
            {
                Text = "Caelia's expression shifts immediately - surprise gives way to grim satisfaction. \"'Ordo Dissolutus'...\" She exhales slowly. \"Then Quintus was right. You found his research.\"<br><br>She takes your arm gently, guiding you to a quieter corner of the temple. \"Quintus and I have been tracking this cult for decades - since before the guild fell. He was once a member himself, you know. Before politics called him to Aevoria.\" Her luminous eyes bore into yours.<br><br>\"The Dissolved Order worships entropy and decay. They believe the Empire is a corruption that must be... undone. For years, we've gathered fragments - intercepted letters, translated texts, whispers in the dark. Quintus handles the political intelligence from the capital. I use the temple's network to track their movements in the provinces.\"<br><br>She lowers her voice. \"We found their hideout months ago. An old smuggler's cave in the Hircinian Forest, repurposed for darker purposes. But we needed proof, evidence we could bring before the Emperor.\" She meets your eyes. \"That's where you come in. If you have the passphrase, you can infiltrate their sanctuary and find what we need.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Where exactly in the forest is this entrance?\"", nextNodeID = "forest_location" }
                }
            });

            priestess.Dialogue.Add("forest_location", new DialogueNode()
            {
                Text = "\"Deep in the eastern reaches of the Hircinian Forest. You'll know it when you see it - a cave entrance with guards posted. They'll challenge anyone who approaches.\" She places a hand on your shoulder, her touch surprisingly warm.<br><br>\"Speak the passphrase, and they'll let you pass. But once inside...\" She pauses, her expression grave. \"Quintus and I have lost agents trying to infiltrate them. You'll be on your own, but you're a guild master. You have resources we never did.\"<br><br>She squeezes your shoulder. \"When you find evidence of their plans - documents, correspondence, anything - bring it to Quintus in Aevoria. He'll know what to do with it. Together, we'll finally expose this threat.\" Her voice drops to a whisper. \"May Keius light your path through the darkness ahead. You'll need all the light you can get.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Thank you, Caelia. I'll be careful.\"", nextNodeID = "end" }
                }
            });

            priestess.Dialogue.Add("end", new DialogueNode ()
            {
                Text = "\"May Keius light your path.\"",
                Choices = { }
            });


            NPC farmer = new NPC();
            farmer.Name = "Gaius";
            farmer.Description = "A burly man of over two meters leans against one of the four posts of his small stall.  His olive skin is deeply tanned from years of tending to his farm in the sun.  As he notices you, he regards you with a mixture of kindness and mild surprise.";
            farmer.ShortDescription = "A farmer";

            // First time meeting Gaius
            farmer.Dialogue.Add("first_greeting", new DialogueNode()
            {
                Text = "As you approach the farmer's stand, he rises to meet you with a smile.<br><br>\"Greetings, friend. Haven't seen you 'round these parts before. Name's Gaius.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Greetings. {player.name}, and I actually have no idea where 'these parts' even are. Where am I?\"", nextNodeID = "ask_about_area" },
                    new DialogueNode.Choice { choiceText = "\"I should get going.\"", nextNodeID = "end" }
                }
            });

            // Subsequent meetings with Gaius
            farmer.Dialogue.Add("repeat_greeting", new DialogueNode()
            {
                Text = "Gaius looks up from arranging his wares and greets you with a familiar smile.<br><br>\"Good to see you again, {player.name}. What can I do for you?\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"I cleared out the bandits from your farm.\"",
                        nextNodeID = "bandits_cleared",
                        RequireNotDiscussedNode = "bandits_cleared"  // Only show once
                    },
                    new DialogueNode.Choice {
                        choiceText = "\"Could you tell me more about this forest and the people you've seen going into it?\"",
                        nextNodeID = "ask_about_forest",
                        RequireNotDiscussedNode = "ask_about_forest"  // Only show if not already discussed
                    },
                    new DialogueNode.Choice {
                        choiceText = "\"Tell me about that closed gate again?\"",
                        nextNodeID = "ask_about_gate",
                        RequireNotDiscussedNode = "ask_about_gate"  // Only show if not already discussed
                    },
                    new DialogueNode.Choice {
                        choiceText = "\"Just passing through. Take care, Gaius.\"",
                        nextNodeID = "end"
                    }
                }
            });

            // Keep original greeting for backward compatibility / fallback
            farmer.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "As you approach the farmer's stand, Gaius rises to meet you with a smile. \"Greetings, friend. Haven't seen you 'round these parts before. Name's Gaius.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Greetings. {player.name}, and I actually have no idea where 'these parts' even are. Where am I?\"", nextNodeID = "ask_about_area" },
                    new DialogueNode.Choice { choiceText = "\"I should get going.\"", nextNodeID = "end" }
                }
            });

            farmer.Dialogue.Add("ask_about_area", new DialogueNode()
            {
                Text = "Gaius briefly raises an eyebrow at this, but a smile quickly replaces the suspicious look on his face. \"Well, in that case, welcome to Belum.\"<br><br>\"The town itself is behind those fortress walls to the north. My farm is a ways to the west, before you reach Mount Gelus. Hircinian Forest is east of here, although you'd be better off avoiding it. Strange folk have been coming and going from there of late.\"<br><br>\"I saw you come from the south, so I imagine you saw that old guild hall. I think it's been abandoned for a while now.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I did come from there, but it's not exactly abandoned any more.   It's mine now, apparently.\"", nextNodeID = "explain_guild" },
                    new DialogueNode.Choice { choiceText = "\"I should get going.\"", nextNodeID = "end" }
                }
            });

            farmer.Dialogue.Add("explain_guild", new DialogueNode()
            {
                Text = "At this, Gaius makes no more attempts to hide his surprise. \"Now that's not something I expected to hear! I'm not sure if I should offer congratulations or condolences.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Too early to say, I think.  I've been tasked with rebuilding the Adventurer's Guild, which means I need to start recruiting.  I suppose I should head into town and see if I can find anyone interested.\"", nextNodeID = "recruit" },
                    new DialogueNode.Choice { choiceText = "Talk about something else.", nextNodeID = "other_topics"}
                }
            });

            farmer.Dialogue.Add("other_topics", new DialogueNode()
            {
                Text = "Gaius nods. \"What can I help you with?\"",

                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"You said the gate to town was closed.  Permanently?\"", nextNodeID = "ask_about_gate" },
                    new DialogueNode.Choice { choiceText = "\"Could you tell me more about this forest and the people you've seen going into it?\"", nextNodeID = "ask_about_forest" }
                }
            });

            farmer.Dialogue.Add("ask_about_gate", new DialogueNode()
            {
                Text = "Gaius gives a small frown. \"The sentry can tell you more, but it seems to me that they're nervous about leaving that gate open given all of the unusual activity down this way. Bandits, mostly, but also whatever's going on in the forest.\"<br><br>\"If I'm being honest, I hope they can resolve it quickly. I'm uncomfortable with those bandits holing up so close to my farm, and there's not enough traffic through these crossroads for me to make a living.\"<br><br>\"If you think you can help, talk to Marcus at the gate.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I might just do that.  I wanted to ask about something else.\"", nextNodeID = "other_topics"}
                }
            });


            farmer.Dialogue.Add("recruit", new DialogueNode()
            {
                Text = "Gaius sighs. \"Well, I've got bad news. The south gate is closed, which is why I'm out here. Normally I'd head into the market to sell my goods, but with the gate being closed I'm forced to set up shop out here.\"<br><br>\"There's a north gate and a west gate, but you'll have a hard time getting to either of them.\"<br><br>\"Bandits have holed up in the caves to the west, and all sorts of foul creatures live in the mountains.\"<br><br>\"I hate to say it, but if you're looking for anyone to help you clear out monsters or bandits, maybe your best bet is to head into that nasty old forest after all.\"",

                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Could you tell me more about this forest and the people you've seen going into it?\"", nextNodeID = "ask_about_forest" }
                }
            });

            farmer.Dialogue.Add("ask_about_forest", new DialogueNode()
            {
                Text = "Gaius shakes his head. \"Dark woods, full of wolves and worse things. Lost a few chickens to whatever lurks in there.\"<br><br>\"Lately a lot of horse-drawn carts have been headed in to the woods. A couple of them stopped here and bought some food, although part of me wished they hadn't. Unsettling folk, every one of them.\"<br><br>\"The last two weren't so bad, though. A venator passed through, and said he'd be back this way once he found whatever it is that he's looking for. Besides him, a soldier went into the forest just this morning. Friendly enough, but had a crazed look in his eye. Seemed like he was looking for someone or something to fight.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Interesting.  It sounds like the woods are my best bet, then.  Maybe one of these interesting characters will join up with me.\"", nextNodeID = "look_for_recruits" }
                }
            });

            farmer.Dialogue.Add("look_for_recruits", new DialogueNode()
            {
                Text = "Gaius smiles at that. \"Ha! Maybe they will. Be safe in there, friend, and good luck finding those recruits.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Thank you. With luck, you'll see me coming back through here soon.\"", nextNodeID = "end"}
                }
            });

            farmer.Dialogue.Add("bandits_cleared", new DialogueNode()
            {
                Text = "Gaius' eyes widen in amazement. \"You what?! The bandits are gone?\" He rushes around the stand and grips your shoulders with both hands. \"{player.name}, you have no idea what this means to me! Those bastards have been terrorizing my farm for weeks!\"<br><br>He releases you and reaches into his coin purse. \"Here, please take this. It's all I can offer right now, but you've saved my livelihood. My family can finally return home safely!\"",
                Action = new DialogueAction {
                    Type = "give_gold",
                    Parameters = { { "amount", "100" } }
                },
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Happy to help, Gaius. Your farm is safe now.\"", nextNodeID = "end" }
                }
            });

            farmer.Dialogue.Add("end", new DialogueNode()
            {
                Text = "Gaius nods warmly. \"Alright, don't let me keep you.<br><br>If you ever end up west of here, stop by my farm some time.\"",
                Choices = { } // empty = conversation ends
            });


            NPC ranger = new NPC();
            ranger.Name = "Silvacis";
            ranger.Description = "A tall, slender man with light hair and a green cloak is here, digging through the mud rather frantically.  Suddenly, his head raises and turns towards you with a scowl.";
            ranger.ShortDescription = "A venator";

            // Silvacis dialogue restructure

            // First time meeting Silvacis
            ranger.Dialogue.Add("first_greeting", new DialogueNode()
            {
                Text = "Silvacis looks up suddenly, his eyes narrowing. \"Damn it, don't sneak up on me like that!\"<br><br>After a moment, the venator regains his composure.\"Apologies, I've been searching for something and haven't had much luck.  I certainly didn't expect to see someone standing over me, but I should be paying more attention.\"<br><br>\"I'm Silvacis.  What brings you out into these woods?\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I'm rebuilding the Adventurer's Guild nearby.\"", nextNodeID = "mention_guild_first" },
                    new DialogueNode.Choice { choiceText = "\"Just exploring the area.\"", nextNodeID = "exploring" }
                }
            });

            // Subsequent meetings with Silvacis
            ranger.Dialogue.Add("repeat_greeting", new DialogueNode()
            {
                Text = "Silvacis looks up from his search and nods in recognition. \"Ah, it's you again.\"",
                Choices =
                {
                    // Show amulet option if discussed via EITHER path (duplicate choices, only one will show)
                    new DialogueNode.Choice {
                        choiceText = "\"Still looking for that amulet?\"",
                        nextNodeID = "main_hub",
                        RequireDiscussedNode = "mention_guild_first"  // Show if discussed via guild path
                    },
                    new DialogueNode.Choice {
                        choiceText = "\"Still looking for that amulet?\"",
                        nextNodeID = "main_hub",
                        RequireDiscussedNode = "exploring"  // Show if discussed via exploring path
                    },
                    new DialogueNode.Choice {
                        choiceText = "\"Just passing through. Good luck with your search.\"",
                        nextNodeID = "end"
                    }
                }
            });

            ranger.Dialogue.Add("mention_guild_first", new DialogueNode()
            {
                Text = "Silvacis' eyes widen with interest. \"The old guild hall? Interesting... I could use the help of a guild actually. The lost item I mentioned - it's a weather-worn silver amulet. I dropped it somewhere in this cursed forest and I can't find it anywhere!\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "You pull the amulet out of your satchel and present it to Silvacis.  \"Is this it?\"",
                        nextNodeID = "give_amulet_guild_known",
                        IsAvailable = (inventory) => inventory.Contains("amulet"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "amulet"} } }
                    },
                    new DialogueNode.Choice { choiceText = "\"What's so special about this amulet?\"", nextNodeID = "amulet_importance" },
                    new DialogueNode.Choice { choiceText = "\"I'll keep an eye out for it.\"", nextNodeID = "will_search" }
                }
            });

            ranger.Dialogue.Add("exploring", new DialogueNode()
            {
                Text = "Silvacis nods gravely. \"Be careful then. These woods are dangerous... Actually, since you're here, maybe you can help me. I've lost something important - a weather-worn silver amulet. Have you seen it?\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "Is this it? (Give amulet)",
                        nextNodeID = "give_amulet_no_guild",
                        IsAvailable = (inventory) => inventory.Contains("amulet"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "amulet"} } }
                    },
                    new DialogueNode.Choice { choiceText = "\"What kind of amulet?\"", nextNodeID = "describe_amulet_no_guild" },
                    new DialogueNode.Choice { choiceText = "\"Sorry, haven't seen it.\"", nextNodeID = "end" }
                }
            });

            ranger.Dialogue.Add("give_amulet_guild_known", new DialogueNode()
            {
                Text = "Silvacis' face lights up with joy as he clutches the amulet. \"That's it! That's my amulet! Thank you so much!\" He pauses, considering. \"You know what? I've been alone and without purpose for too long. A guild could be exactly what I need.\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"It is good to have you, Silvacis.\"",
                        nextNodeID = "recruit_offer"
                    }
                }
            });

            ranger.Dialogue.Add("give_amulet_no_guild", new DialogueNode()
            {
                Text = "Silvacis takes the amulet reverently, his eyes glistening. \"That's it! That's my amulet! Thank you so much! I owe you a great debt...\" He looks at you appraisingly. \"Say, you seem capable. I don't suppose you're looking for companions? I'm a skilled ranger, and I've been thinking it's time to leave these woods.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Actually, I was very recently tasked with rebuilding the Adventurer's Guild. Interested?\"", nextNodeID = "reveal_guild" },
                }
            });

            ranger.Dialogue.Add("reveal_guild", new DialogueNode()
            {
                Text = "Silvacis straightens with renewed purpose. \"It's been too long since I had companions or a real purpose.  Count me in.\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"It is good to have you, Silvacis.\"",
                        nextNodeID = "recruit_offer"
                    }
                }
            });

            ranger.Dialogue.Add("amulet_importance", new DialogueNode()
            {
                Text = "Silvacis' expression becomes somber. \"It belonged to someone important to me. It's all I have left of them... Please, if you find it, bring it to me.\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "You pull the amulet out of your satchel and present it to Silvacis.  \"Is this it?\"",
                        nextNodeID = "give_amulet_guild_known",
                        IsAvailable = (inventory) => inventory.Contains("amulet"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "amulet"} } }
                    },
                    new DialogueNode.Choice { choiceText = "\"I'll look for it.\"", nextNodeID = "will_search" }
                }
            });

            ranger.Dialogue.Add("will_search", new DialogueNode()
            {
                Text = "Silvacis nods gratefully. \"Thank you. I'll keep searching here. If you find it, please bring it to me.\"",
                Choices = { }
            });

            // Main hub for return visits
            ranger.Dialogue.Add("main_hub", new DialogueNode()
            {
                Text = "Silvacis looks up hopefully. \"Have you found my amulet yet? It's a weather-worn silver piece, very important to me.\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "You pull the amulet out of your satchel and present it to Silvacis. \"Is this it?\"",
                        nextNodeID = "give_amulet_return",
                        IsAvailable = (inventory) => inventory.Contains("amulet"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "amulet"} } }
                    },
                    new DialogueNode.Choice { choiceText = "\"Still looking for it.\"", nextNodeID = "end" }
                }
            });

            ranger.Dialogue.Add("give_amulet_return", new DialogueNode()
            {
                Text = "Silvacis takes the amulet with trembling hands. \"My amulet! Thank you!\" He clutches it to his chest for a moment, then looks at you with determination. \"You know, I've been thinking about what you said about that guild. I'd like to join, if you'll have me.\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"It is good to have you, Silvacis.\"",
                        nextNodeID = "recruit_offer"
                    }
                }
            });

            // Keep the existing recruit_offer
            ranger.Dialogue.Add("recruit_offer", new DialogueNode()
            {
                Text = "Silvacis shoulders his pack with renewed energy. \"Lead on, Guild Master. Oh, and if you're looking for more recruits, I heard there's a fighter who camps deeper in these woods - bit of a hermit, but skilled with a blade.\"",
                Action = new DialogueAction { Type = "add_recruit", Parameters = { { "class", "Venator" } } },
                Choices = { }
            });

            // Terminal node - automatically resets to greeting when conversation ends
            ranger.Dialogue.Add("end", new DialogueNode()
            {
                Text = "Silvacis returns to his search, his movements methodical but desperate. \"Please, if you find it, let me know. I'll be searching around here.\"",
                Choices = { } // conversation ends and resets
            });

            // And "describe_amulet_no_guild" that was also referenced:
            ranger.Dialogue.Add("describe_amulet_no_guild", new DialogueNode()
            {
                Text = "Silvacis' voice becomes quieter. \"It's a weather-worn silver amulet, quite old. It belonged to someone important to me. It's all I have left of them.\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "Is this it? (Give amulet)",
                        nextNodeID = "give_amulet_no_guild",
                        IsAvailable = (inventory) => inventory.Contains("amulet"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "amulet"} } }
                    },
                    new DialogueNode.Choice { choiceText = "\"I'll keep an eye out for it.\"", nextNodeID = "end" }
                }
            });

            // Lucius - Quest Recruit (Raid Bandit Camp)
            NPC lucius = new NPC();
            lucius.Name = "Lucius";
            lucius.Description = "A battle-hardened warrior with scars from countless fights. He wields a massive two-handed sword with practiced ease.";
            lucius.ShortDescription = "Lucius";
            lucius.IsHostile = false;
            lucius.Class = new Legionnaire(); // Tough frontline fighter
            lucius.Class.ApplyClassBonuses(lucius);
            npcs.Add(lucius.Name, lucius);

            // Grax - Quest Recruit (Mountain Rescue)
            NPC grax = new NPC();
            grax.Name = "Grax";
            grax.Description = "A grizzled mountain hunter with keen eyes and a weather-worn bow. He moves with the quiet confidence of someone who's survived years in the wilderness.";
            grax.ShortDescription = "Grax";
            grax.IsHostile = false;
            grax.Class = new Venator(); // Skilled ranged fighter
            grax.Class.ApplyClassBonuses(grax);
            npcs.Add(grax.Name, grax);

            // Cassia - Quest Recruit (Cultist Investigation)
            NPC cassia = new NPC();
            cassia.Name = "Cassia";
            cassia.Description = "A mysterious Oracle draped in flowing robes, her eyes gleaming with ancient knowledge. She carries a gnarled staff adorned with mystical runes.";
            cassia.ShortDescription = "Cassia";
            cassia.IsHostile = false;
            cassia.Class = new Oracle(); // Spellcaster and healer
            cassia.Class.ApplyClassBonuses(cassia);
            npcs.Add(cassia.Name, cassia);

            // Felix - Quest Recruit (Goblin Siege)
            NPC felix = new NPC();
            felix.Name = "Felix";
            felix.Description = "A young but determined Legionnaire with polished armor and a sharp gladius. His disciplined stance speaks of rigorous training.";
            felix.ShortDescription = "Felix";
            felix.IsHostile = false;
            felix.Class = new Legionnaire(); // Defensive warrior
            felix.Class.ApplyClassBonuses(felix);
            npcs.Add(felix.Name, felix);

            NPC bandit = new NPC();
            bandit.Name = "Bandit";
            bandit.Description = "A rough-looking bandit blocks your path, weapon drawn.";
            bandit.ShortDescription = "A hostile bandit";
            bandit.IsHostile = true;
            bandit.Health = 8;
            bandit.MaxHealth = 8;
            bandit.AttackDamage = 2;
            bandit.MinGold = 1;
            bandit.MinGold = 3;
            bandit.ExperienceReward = 25;
            bandit.LootTable = new Dictionary<string, int>
            {
                {"potion", 30},      // 30% chance
                {"iron gladius", 5}    // 5% chance  
            };



            NPC banditThug = new NPC();
            banditThug.Name = "Bandit Thug";
            banditThug.Description = "A rougher-looking bandit blocks your path, weapons drawn.";
            banditThug.ShortDescription = "A hostile bandit thug";
            banditThug.IsHostile = true;
            banditThug.Health = 12;
            banditThug.MaxHealth = 12;
            banditThug.AttackDamage = 5;
            banditThug.MinGold = 2;
            banditThug.MaxGold = 4;
            banditThug.ExperienceReward = 40;
            banditThug.LootTable = new Dictionary<string, int>
            {
                {"potion", 40},      // 40% chance
                {"iron gladius", 10}   // 10% chance
            };

            NPC banditLeader = new NPC();
            banditLeader.Name = "Bandit Leader";
            banditLeader.Description = "A roughest-looking bandit blocks your path, weapon and shield drawn.";
            banditLeader.ShortDescription = "A hostile bandit leader";
            banditLeader.IsHostile = true;
            banditLeader.Health = 18;
            banditLeader.MaxHealth = 18;
            banditLeader.AttackDamage = 4;
            banditLeader.Defense = 1;
            banditLeader.MinGold = 3;
            banditLeader.MaxGold = 5;
            banditLeader.ExperienceReward = 60;
            banditLeader.Energy = 10;
            banditLeader.MaxEnergy = 10;
            banditLeader.EnergyRegenPerTurn = 2;
            banditLeader.Role = EnemyRole.Melee;
            banditLeader.AbilityNames.Add("Power Attack");
            banditLeader.LootTable = new Dictionary<string, int>
            {
                {"potion", 50},      // 50% chance
                {"battle axe", 20},  // 20% chance
                {"iron gladius", 15}   // 15% chance
            };

            NPC direWolf = new NPC();
            direWolf.Name = "Dire Wolf";
            direWolf.Description = "A huge dire wolf bares its fangs and snarls at you.";
            direWolf.ShortDescription = "A hostile dire wolf";
            direWolf.IsHostile = true;
            direWolf.Health = 19;
            direWolf.MaxHealth = 19;
            direWolf.AttackDamage = 6;
            direWolf.MinGold = 2;
            direWolf.MaxGold = 4;
            direWolf.ExperienceReward = 35;


            NPC fighter = new NPC();
            fighter.Name = "Braxus";
            fighter.Description = "A grizzled warrior in worn leather armor, practicing sword forms with intense focus. Scars tell the story of countless battles.";
            fighter.ShortDescription = "A lone fighter";
            fighter.IsHostile = false;
            fighter.Health = 15;
            fighter.MaxHealth = 15;
            fighter.AttackDamage = 3;
            fighter.Defense = 1;
            fighter.Speed = 1;
            fighter.MinGold = 0;
            fighter.MaxGold = 0;
            fighter.RecruitableAfterDefeat = true;
            fighter.RecruitClass = "Legionnaire";
            fighter.YieldDialogue = "You fight well. Perhaps... perhaps I was wrong about you.";
            fighter.AcceptDialogue = "I'll head to your guild hall. You've earned my respect.";

            fighter.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "Braxus stops mid-swing and turns to face you, his expression hard. \"Leave me be, stranger. I came to these woods for solitude, not conversation.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I'm rebuilding the Adventurer's Guild. Interested?\"", nextNodeID = "reject_guild" },
                    new DialogueNode.Choice { choiceText = "\"Fair enough. I'll leave you alone.\"", nextNodeID = "goodbye" }
                }
            });

            fighter.Dialogue.Add("reject_guild", new DialogueNode()
            {
                Text = "Braxus laughs bitterly. \"Ha! I'm done taking orders. If you want me to join anything, you'll have to prove you're worth following. Draw your weapon!\"",
                Action = new DialogueAction { Type = "trigger_combat" },
                Choices = { }
            });

            fighter.Dialogue.Add("goodbye", new DialogueNode()
            {
                Text = "Braxus grunts and returns to his practice. \"Smart choice. Now leave me be.\"",
                Choices = { }
            });

            // Mountain Path NPCs - High XP for testing
            NPC mountainBandit = new NPC();
            mountainBandit.Name = "Mountain Bandit";
            mountainBandit.Description = "A hardened bandit wielding twin daggers, scarred from countless mountain raids.";
            mountainBandit.ShortDescription = "A mountain bandit";
            mountainBandit.IsHostile = true;
            mountainBandit.Health = 25;
            mountainBandit.MaxHealth = 25;
            mountainBandit.AttackDamage = 6;
            mountainBandit.Defense = 1;
            mountainBandit.Speed = 3;
            mountainBandit.MinGold = 5;
            mountainBandit.MaxGold = 10;
            mountainBandit.ExperienceReward = 100;
            mountainBandit.LootTable = new Dictionary<string, int>
{
    {"potion", 50},
    {"energy potion", 30}
};

            NPC frostWolf = new NPC();
            frostWolf.Name = "Frost Wolf";
            frostWolf.Description = "A massive wolf with ice-white fur and frost forming on its breath.";
            frostWolf.ShortDescription = "A frost wolf";
            frostWolf.IsHostile = true;
            frostWolf.Health = 30;
            frostWolf.MaxHealth = 30;
            frostWolf.AttackDamage = 8;
            frostWolf.Defense = 0;
            frostWolf.Speed = 4;
            frostWolf.MinGold = 3;
            frostWolf.MaxGold = 8;
            frostWolf.ExperienceReward = 150;
            frostWolf.LootTable = new Dictionary<string, int>
{
    {"potion", 60},
    {"energy potion", 40}
};

            NPC iceElemental = new NPC();
            iceElemental.Name = "Ice Elemental";
            iceElemental.Description = "A swirling mass of ice and snow, barely holding a humanoid form.";
            iceElemental.ShortDescription = "An ice elemental";
            iceElemental.IsHostile = true;
            iceElemental.Health = 35;
            iceElemental.MaxHealth = 35;
            iceElemental.AttackDamage = 10;
            iceElemental.Defense = 2;
            iceElemental.Speed = 2;
            iceElemental.MinGold = 8;
            iceElemental.MaxGold = 15;
            iceElemental.ExperienceReward = 200;
            iceElemental.Role = EnemyRole.Ranged;  // Magical attacks from range
            iceElemental.LootTable = new Dictionary<string, int>
{
    {"potion", 70},
    {"energy potion", 50},
    {"restoration scroll", 20}
};

            NPC thunderEagle = new NPC();
            thunderEagle.Name = "Thunder Eagle";
            thunderEagle.Description = "A massive eagle crackling with lightning, its eyes glowing with electrical energy.";
            thunderEagle.ShortDescription = "A thunder eagle";
            thunderEagle.IsHostile = true;
            thunderEagle.Health = 40;
            thunderEagle.MaxHealth = 40;
            thunderEagle.AttackDamage = 12;
            thunderEagle.Defense = 1;
            thunderEagle.Speed = 5;
            thunderEagle.MinGold = 10;
            thunderEagle.MaxGold = 20;
            thunderEagle.ExperienceReward = 250;
            thunderEagle.Role = EnemyRole.Ranged;  // Flying creature with lightning attacks
            thunderEagle.LootTable = new Dictionary<string, int>
{
    {"potion", 80},
    {"energy potion", 60},
    {"restoration scroll", 30}
};

            NPC mountainWarlord = new NPC();
            mountainWarlord.Name = "Mountain Warlord";
            mountainWarlord.Description = "A towering warrior clad in ancient armor, wielding a massive two-handed sword. Runes glow faintly on the blade.";
            mountainWarlord.ShortDescription = "A mountain warlord";
            mountainWarlord.IsHostile = true;
            mountainWarlord.Health = 80;
            mountainWarlord.MaxHealth = 80;
            mountainWarlord.AttackDamage = 15;
            mountainWarlord.Defense = 4;
            mountainWarlord.Speed = 2;
            mountainWarlord.MinGold = 30;
            mountainWarlord.MaxGold = 50;
            mountainWarlord.ExperienceReward = 500;
            mountainWarlord.LootTable = new Dictionary<string, int>
{
    {"potion", 100},
    {"energy potion", 80},
    {"restoration scroll", 50},
    {"battle axe", 40}
};

            NPC eliteGuard = new NPC();
            eliteGuard.Name = "Elite Guard";
            eliteGuard.Description = "A disciplined warrior in polished armor, standing at attention near the warlord.";
            eliteGuard.ShortDescription = "An elite guard";
            eliteGuard.IsHostile = true;
            eliteGuard.Health = 45;
            eliteGuard.MaxHealth = 45;
            eliteGuard.AttackDamage = 10;
            eliteGuard.Defense = 3;
            eliteGuard.Speed = 3;
            eliteGuard.MinGold = 10;
            eliteGuard.MaxGold = 20;
            eliteGuard.ExperienceReward = 200;
            eliteGuard.LootTable = new Dictionary<string, int>
{
    {"potion", 70},
    {"energy potion", 50},
    {"iron gladius", 30}
};

            // Add all new NPCs to the dictionary
            npcs.Add(mountainBandit.Name, mountainBandit);
            npcs.Add(frostWolf.Name, frostWolf);
            npcs.Add(iceElemental.Name, iceElemental);
            npcs.Add(thunderEagle.Name, thunderEagle);
            npcs.Add(mountainWarlord.Name, mountainWarlord);
            npcs.Add(eliteGuard.Name, eliteGuard);

            npcs.Add(farmer.Name, farmer);
            npcs.Add(ranger.Name, ranger);
            npcs.Add(bandit.Name, bandit);
            npcs.Add(banditThug.Name, banditThug);
            npcs.Add(banditLeader.Name, banditLeader);
            npcs.Add(fighter.Name, fighter);
            npcs.Add(direWolf.Name, direWolf);
            npcs.Add(priestess.Name, priestess);

            // ===== BANDIT CAVE NPCS =====

            // Bandit Scout - Early cave enemy (Level 4)
            NPC banditScout = new NPC();
            banditScout.Name = "Bandit Scout";
            banditScout.Description = "A wiry bandit in leather armor, alert and quick on their feet. They carry a short bow and dagger.";
            banditScout.ShortDescription = "A bandit scout";
            banditScout.IsHostile = true;
            banditScout.Health = 35;
            banditScout.MaxHealth = 35;
            banditScout.Energy = 20;
            banditScout.MaxEnergy = 20;
            banditScout.AttackDamage = 6;
            banditScout.Defense = 3;
            banditScout.Speed = 12;
            banditScout.DamageCount = 1;
            banditScout.DamageDie = 6;
            banditScout.DamageBonus = 4;
            banditScout.MinGold = 5;
            banditScout.MaxGold = 12;
            banditScout.ExperienceReward = 40;
            banditScout.Role = EnemyRole.Ranged;
            banditScout.IsBackRow = true;
            banditScout.EnergyRegenPerTurn = 2;
            banditScout.AbilityNames.Add("Piercing Arrow");
            banditScout.LootTable = new Dictionary<string, int> { {"potion", 30} };

            // Bandit Cutthroat - Mid cave enemy (Level 5)
            NPC banditCutthroat = new NPC();
            banditCutthroat.Name = "Bandit Cutthroat";
            banditCutthroat.Description = "A dangerous-looking bandit with multiple scars and a cruel sneer. Dual daggers hang at their belt.";
            banditCutthroat.ShortDescription = "A bandit cutthroat";
            banditCutthroat.IsHostile = true;
            banditCutthroat.Health = 45;
            banditCutthroat.MaxHealth = 45;
            banditCutthroat.Energy = 25;
            banditCutthroat.MaxEnergy = 25;
            banditCutthroat.AttackDamage = 8;
            banditCutthroat.Defense = 4;
            banditCutthroat.Speed = 14;
            banditCutthroat.DamageCount = 1;
            banditCutthroat.DamageDie = 8;
            banditCutthroat.DamageBonus = 6;
            banditCutthroat.MinGold = 8;
            banditCutthroat.MaxGold = 18;
            banditCutthroat.ExperienceReward = 55;
            banditCutthroat.Role = EnemyRole.Melee;
            banditCutthroat.LootTable = new Dictionary<string, int> { {"potion", 40}, {"energy potion", 20} };

            // Bandit Enforcer - Late cave enemy (Level 6)
            NPC banditEnforcer = new NPC();
            banditEnforcer.Name = "Bandit Enforcer";
            banditEnforcer.Description = "A heavily armored bandit wielding a large axe. This one looks like they've seen many battles and won most of them.";
            banditEnforcer.ShortDescription = "A bandit enforcer";
            banditEnforcer.IsHostile = true;
            banditEnforcer.Health = 60;
            banditEnforcer.MaxHealth = 60;
            banditEnforcer.Energy = 30;
            banditEnforcer.MaxEnergy = 30;
            banditEnforcer.AttackDamage = 10;
            banditEnforcer.Defense = 6;
            banditEnforcer.Speed = 10;
            banditEnforcer.DamageCount = 1;
            banditEnforcer.DamageDie = 10;
            banditEnforcer.DamageBonus = 8;
            banditEnforcer.MinGold = 12;
            banditEnforcer.MaxGold = 25;
            banditEnforcer.ExperienceReward = 75;
            banditEnforcer.Role = EnemyRole.Melee;
            banditEnforcer.LootTable = new Dictionary<string, int> { {"potion", 50}, {"energy potion", 30}, {"battle axe", 15} };

            // Bandit Guard - Deep Cave Guard with Iron Key (Level 6, Elite)
            NPC banditGuard = new NPC();
            banditGuard.Name = "Bandit Guard";
            banditGuard.Description = "A heavily armored bandit stands guard, clearly the most formidable warrior in this section of the cave. A large iron key hangs from his belt alongside a wicked-looking axe.";
            banditGuard.ShortDescription = "A bandit guard";
            banditGuard.IsHostile = true;
            banditGuard.Health = 80;  // Increased from 60
            banditGuard.MaxHealth = 80;
            banditGuard.Energy = 35;  // Increased from 30
            banditGuard.MaxEnergy = 35;
            banditGuard.AttackDamage = 12;  // Increased from 10
            banditGuard.Defense = 7;  // Increased from 6
            banditGuard.Speed = 11;  // Increased from 10
            banditGuard.DamageCount = 1;
            banditGuard.DamageDie = 12;  // Increased from d10 to d12
            banditGuard.DamageBonus = 10;  // Increased from 8
            banditGuard.MinGold = 15;
            banditGuard.MaxGold = 30;
            banditGuard.ExperienceReward = 100;  // Increased from 75
            banditGuard.Role = EnemyRole.Melee;
            banditGuard.LootTable = new Dictionary<string, int> { {"iron key", 100}, {"potion", 60}, {"energy potion", 40}, {"battle axe", 20} };

            // Bandit Warlord - Boss (Level 7)
            NPC banditWarlord = new NPC();
            banditWarlord.Name = "Bandit Warlord";
            banditWarlord.Description = "The bandit leader stands before you, clad in stolen armor and wielding a wicked-looking blade. His eyes gleam with malice and greed. This is the Warlord who has terrorized the region.";
            banditWarlord.ShortDescription = "The Bandit Warlord";
            banditWarlord.PreCombatDialogue = "As you step into the chamber, the Bandit Warlord rises from his makeshift throne as his enforcers on either side grab their weapons and approach you.  The Warlord eyes you with a cruel grin on his face, then speaks: \"So, you've come to take all of this from me?  Let's get this over with.\"";
            banditWarlord.IsHostile = true;
            banditWarlord.Health = 85;
            banditWarlord.MaxHealth = 85;
            banditWarlord.Energy = 40;
            banditWarlord.MaxEnergy = 40;
            banditWarlord.AttackDamage = 12;
            banditWarlord.Defense = 8;
            banditWarlord.Speed = 12;
            banditWarlord.DamageCount = 2;
            banditWarlord.DamageDie = 8;
            banditWarlord.DamageBonus = 10;
            banditWarlord.MinGold = 50;
            banditWarlord.MaxGold = 100;
            banditWarlord.ExperienceReward = 150;
            banditWarlord.Role = EnemyRole.Melee;
            banditWarlord.EnergyRegenPerTurn = 2;
            banditWarlord.AbilityNames.Add("Power Attack");
            banditWarlord.AbilityNames.Add("Cleave");
            banditWarlord.LootTable = new Dictionary<string, int>
            {
                {"warlord's head", 100},  // Quest item - always drops
                {"indecipherable letter", 100},  // Quest item - always drops
                {"steel gladius", 30},
                {"chainmail", 25},
                {"potion", 60},
                {"greater potion", 40}
            };

            // Livia - Recruitable Venator (Hunter/Archer class)
            NPC livia = new NPC();
            livia.Name = "Livia";
            livia.Description = "A skilled hunter with a hunter's bow slung across her back. She eyes you warily, hand near her quiver. Her stance suggests she's been fighting for survival in these caves.";
            livia.ShortDescription = "A woman with a bow";
            livia.IsHostile = true;  // Initially hostile
            livia.Health = 50;
            livia.MaxHealth = 50;
            livia.Energy = 30;
            livia.MaxEnergy = 30;
            livia.AttackDamage = 9;
            livia.Defense = 4;
            livia.Speed = 13;
            livia.DamageCount = 1;
            livia.DamageDie = 8;
            livia.DamageBonus = 7;
            livia.MinGold = 10;
            livia.MaxGold = 20;
            livia.ExperienceReward = 60;
            livia.Role = EnemyRole.Ranged;
            livia.IsBackRow = true;
            livia.EnergyRegenPerTurn = 2;
            livia.AbilityNames.Add("Piercing Arrow");
            livia.AbilityNames.Add("Covering Shot");
            livia.RecruitableAfterDefeat = true;
            livia.RecruitClass = "Venator";
            livia.YieldDialogue = "Wait! [She lowers her bow] I surrender. I'm not one of these bandits - they captured me weeks ago and forced me to stand guard. Please, let me join you. I'm a skilled hunter and I know how to survive.";
            livia.AcceptDialogue = "Thank the gods. I won't let you down. These bandits will pay for what they've done. [She reaches into her pouch] Here - take this bronze key. The bandits took it from me when they captured me. It unlocks the gate to the Warlord's chamber. You'll need it to reach him.";

            // Add bandit cave NPCs (sentry/Marcus already added earlier)
            npcs.Add(banditScout.Name, banditScout);
            npcs.Add(banditCutthroat.Name, banditCutthroat);
            npcs.Add(banditEnforcer.Name, banditEnforcer);
            npcs.Add(banditGuard.Name, banditGuard);
            npcs.Add(banditWarlord.Name, banditWarlord);
            npcs.Add(livia.Name, livia);

            // ===== TESTING RECRUITS (Hidden in rooms 999-991) =====
            // Test Recruit 1 - Legionnaire, Female, Conversational
            NPC testRecruit1 = new NPC();
            testRecruit1.Name = "Valeria";
            testRecruit1.Description = "A strong woman in battle-worn armor stands at attention, clearly experienced in combat.";
            testRecruit1.ShortDescription = "A veteran warrior";
            testRecruit1.IsHostile = false;
            testRecruit1.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "Greetings! I've been waiting for someone from the guild. Ready to serve!",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Join the guild?", nextNodeID = "recruit_offer" }
                }
            });
            testRecruit1.Dialogue.Add("recruit_offer", new DialogueNode()
            {
                Text = "Absolutely! I'll head to the guild hall immediately.",
                Action = new DialogueAction { Type = "add_recruit", Parameters = { { "class", "Legionnaire" } } },
                Choices = { }
            });

            // Test Recruit 2 - Venator, Male, Conversational
            NPC testRecruit2 = new NPC();
            testRecruit2.Name = "Darius";
            testRecruit2.Description = "A skilled archer with keen eyes scans the area, bow at the ready.";
            testRecruit2.ShortDescription = "A skilled hunter";
            testRecruit2.IsHostile = false;
            testRecruit2.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "Good timing. I was just thinking about joining an adventuring guild.",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Join us?", nextNodeID = "recruit_offer" }
                }
            });
            testRecruit2.Dialogue.Add("recruit_offer", new DialogueNode()
            {
                Text = "Count me in. I'll make my way to the guild hall.",
                Action = new DialogueAction { Type = "add_recruit", Parameters = { { "class", "Venator" } } },
                Choices = { }
            });

            // Test Recruit 3 - Oracle, Female, Conversational
            NPC testRecruit3 = new NPC();
            testRecruit3.Name = "Lyra";
            testRecruit3.Description = "A young woman in flowing robes hums softly, her hands glowing with faint magical energy.";
            testRecruit3.ShortDescription = "A mystic";
            testRecruit3.IsHostile = false;
            testRecruit3.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "The stars told me you'd come. I see a guild in your future... and mine.",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Will you join?", nextNodeID = "recruit_offer" }
                }
            });
            testRecruit3.Dialogue.Add("recruit_offer", new DialogueNode()
            {
                Text = "Destiny calls. I shall follow.",
                Action = new DialogueAction { Type = "add_recruit", Parameters = { { "class", "Oracle" } } },
                Choices = { }
            });

            // Test Recruit 4 - Legionnaire, Male, Combat Recruit
            NPC testRecruit4 = new NPC();
            testRecruit4.Name = "Marcus the Bold";
            testRecruit4.Description = "A proud warrior with a stern expression stands before you, his hand resting on the pommel of his sword. Battle scars mark his arms, and his weathered armor bears the crest of a fallen legion.";
            testRecruit4.ShortDescription = "A proud warrior";
            testRecruit4.IsHostile = false;
            testRecruit4.Health = 10;
            testRecruit4.MaxHealth = 10;
            testRecruit4.AttackDamage = 2;
            testRecruit4.Defense = 1;
            testRecruit4.Speed = 1;
            testRecruit4.MinGold = 0;
            testRecruit4.MaxGold = 0;
            testRecruit4.RecruitableAfterDefeat = true;
            testRecruit4.RecruitClass = "Legionnaire";
            testRecruit4.YieldDialogue = "[He lowers his sword, breathing heavily] You fight with honor. Few have bested me in single combat. The gods have spoken - I was meant to follow you.";
            testRecruit4.AcceptDialogue = "You have my blade, guild master. I swear by Mars himself - I will defend your guild as I once defended the Sixth Legion. Lead on, and I shall follow.";

            testRecruit4.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "Marcus eyes you with a mixture of pride and wariness. \"So, you're the one rebuilding the old Adventurer's Guild. I've heard whispers.\"<br><br>He crosses his arms. \"I served with the Sixth Legion for twenty years. Honor. Discipline. Victory. That legion is gone now - disbanded after our general fell.\" His jaw tightens. \"I won't join just anyone's banner again.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"What would it take to earn your service?\"", nextNodeID = "explain_honor" },
                    new DialogueNode.Choice { choiceText = "\"Your loss. I don't need reluctant recruits.\"", nextNodeID = "offended" }
                }
            });

            testRecruit4.Dialogue.Add("explain_honor", new DialogueNode()
            {
                Text = "Marcus nods approvingly. \"The right question. I seek a leader worthy of following - someone who won't abandon their soldiers when times grow dark.\"<br><br>He draws his gladius, sunlight glinting off the well-maintained blade. \"Prove your strength. Best me in honorable combat, and I'll know you have what it takes. Refuse, and you're not the leader I'm looking for.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I accept your challenge. Draw steel!\"", nextNodeID = "accept_duel" },
                    new DialogueNode.Choice { choiceText = "\"I don't have time for this.\"", nextNodeID = "refuse_duel" }
                }
            });

            testRecruit4.Dialogue.Add("accept_duel", new DialogueNode()
            {
                Text = "\"Good!\" Marcus assumes a combat stance, respect flickering in his eyes. \"Show me the mettle of the new guild master!\"",
                Action = new DialogueAction { Type = "trigger_combat" },
                Choices = { }
            });

            testRecruit4.Dialogue.Add("refuse_duel", new DialogueNode()
            {
                Text = "Marcus's expression hardens with disappointment. \"Then you're not the leader I thought you were. Leave me.\"",
                Choices = { }
            });

            testRecruit4.Dialogue.Add("offended", new DialogueNode()
            {
                Text = "Marcus's eyes flash with anger. \"Arrogant whelp! You'll regret those words!\"<br><br>He draws his sword aggressively. \"I'll teach you some respect!\"",
                Action = new DialogueAction { Type = "trigger_combat" },
                Choices = { }
            });

            // Test Recruit 5 - Venator, Female, Combat Recruit
            NPC testRecruit5 = new NPC();
            testRecruit5.Name = "Aria Swift";
            testRecruit5.Description = "A lithe archer with a cocky grin leans against a tree, her hunter's bow already strung. Her movements are fluid and precise - the mark of someone who's spent years stalking prey through dense wilderness.";
            testRecruit5.ShortDescription = "A swift archer";
            testRecruit5.IsHostile = false;
            testRecruit5.Health = 8;
            testRecruit5.MaxHealth = 8;
            testRecruit5.AttackDamage = 3;
            testRecruit5.Defense = 0;
            testRecruit5.Speed = 2;
            testRecruit5.MinGold = 0;
            testRecruit5.MaxGold = 0;
            testRecruit5.RecruitableAfterDefeat = true;
            testRecruit5.RecruitClass = "Venator";
            testRecruit5.YieldDialogue = "[She laughs breathlessly, lowering her bow] Alright, alright! You're faster than you look. Diana's grace, I haven't had a fight that intense in months!";
            testRecruit5.AcceptDialogue = "You've got my attention - and my bow. Fair warning though: I don't follow slow leaders. Keep up that pace, and we'll get along just fine. See you at the guild hall!";

            testRecruit5.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "Aria's eyes light up as you approach. \"Well, well! Fresh meat walking through my hunting grounds.\" She grins mischievously. \"Wait - I've heard about you. The guild master wannabe, right?\"<br><br>She spins an arrow between her fingers with practiced ease. \"I've been hunting these woods solo for three years. Bandits, beasts, bounties - you name it. Never needed a team before.\" She tilts her head. \"But I'm bored out of my mind. Maybe a guild could be... interesting.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Interested enough to join?\"", nextNodeID = "explain_boredom" },
                    new DialogueNode.Choice { choiceText = "\"A lone wolf, huh? We don't need loners.\"", nextNodeID = "provoked" }
                }
            });

            testRecruit5.Dialogue.Add("explain_boredom", new DialogueNode()
            {
                Text = "\"Ha! Maybe.\" Aria nocks an arrow casually. \"But here's the thing - I'm not joining some amateur operation. I need to know you can keep up.\"<br><br>She grins wolfishly. \"So how about this: a friendly duel. You and me. If you can match my speed, prove you're not just another plodding swordsman... then yeah, I'm in. What do you say, guild master?\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"You're on. Let's dance!\"", nextNodeID = "accept_duel" },
                    new DialogueNode.Choice { choiceText = "\"I don't have time for games.\"", nextNodeID = "refuse_duel" }
                }
            });

            testRecruit5.Dialogue.Add("accept_duel", new DialogueNode()
            {
                Text = "\"Now we're talking!\" Aria takes a few steps back, grinning ear to ear. \"Try to keep up!\"",
                Action = new DialogueAction { Type = "trigger_combat" },
                Choices = { }
            });

            testRecruit5.Dialogue.Add("refuse_duel", new DialogueNode()
            {
                Text = "Aria shrugs, clearly disappointed. \"Boring. Come back when you've got some fire in you.\"<br><br>She turns away, already losing interest.",
                Choices = { }
            });

            testRecruit5.Dialogue.Add("provoked", new DialogueNode()
            {
                Text = "Aria's grin widens dangerously. \"Oh, you did NOT just say that.\" She draws her bow in one smooth motion. \"Let me show you what this 'loner' can do!\"",
                Action = new DialogueAction { Type = "trigger_combat" },
                Choices = { }
            });

            // Test Recruit 6 - Oracle, Male, Combat Recruit
            NPC testRecruit6 = new NPC();
            testRecruit6.Name = "Aldric the Wise";
            testRecruit6.Description = "An elderly mage with silver hair and penetrating eyes studies you carefully. His gnarled staff pulses with barely-contained arcane power, and ancient runes glow faintly on his weathered robes. Despite his age, there's a sharpness to his gaze that suggests a keen and dangerous mind.";
            testRecruit6.ShortDescription = "An old mage";
            testRecruit6.IsHostile = false;
            testRecruit6.Health = 6;
            testRecruit6.MaxHealth = 6;
            testRecruit6.AttackDamage = 4;
            testRecruit6.Defense = 0;
            testRecruit6.Speed = 1;
            testRecruit6.MinGold = 0;
            testRecruit6.MaxGold = 0;
            testRecruit6.RecruitableAfterDefeat = true;
            testRecruit6.RecruitClass = "Oracle";
            testRecruit6.YieldDialogue = "[He leans heavily on his staff, breathing with effort] Remarkable. Your command of the arcane... and more importantly, your restraint in not delivering a killing blow... speaks to both power and wisdom. Perhaps you ARE worth following.";
            testRecruit6.AcceptDialogue = "I shall lend my knowledge to your guild. My days of solitary study grow lonely, and I sense great events approaching. The stars themselves whisper of change. Lead wisely, young one, and I shall stand with you.";

            testRecruit6.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "The old mage regards you with calculating eyes. \"You carry yourself like one touched by destiny.\" His voice is measured, scholarly. \"I am Aldric - once court wizard to House Valerius, now... merely a wanderer.\"<br><br>He taps his staff thoughtfully. \"I've heard whispers of someone rebuilding the Adventurer's Guild. A bold undertaking. Foolish, perhaps... but bold nonetheless.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Would you consider joining our cause?\"", nextNodeID = "explain_caution" },
                    new DialogueNode.Choice { choiceText = "\"Foolish? Watch your tongue, old man.\"", nextNodeID = "insulted" }
                }
            });

            testRecruit6.Dialogue.Add("explain_caution", new DialogueNode()
            {
                Text = "Aldric strokes his beard. \"Direct. I appreciate that.\" He pauses, studying you more intently. \"My last lord squandered my counsel, pursuing glory over wisdom. His house fell to ruin.\"<br><br>\"Before I pledge my knowledge to another... I must know you possess both strength AND judgment. A test, then - one of magical prowess and restraint.\" His staff begins to glow. \"Face me in arcane combat. But know this: how you fight matters as much as whether you win.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I accept your test, Aldric.\"", nextNodeID = "accept_test" },
                    new DialogueNode.Choice { choiceText = "\"I don't have time for tests.\"", nextNodeID = "refuse_test" }
                }
            });

            testRecruit6.Dialogue.Add("accept_test", new DialogueNode()
            {
                Text = "\"Good.\" Aldric's eyes gleam with approval. \"Let us see what you are made of.\"<br><br>Arcane energy crackles around his staff as he takes a defensive stance.",
                Action = new DialogueAction { Type = "trigger_combat" },
                Choices = { }
            });

            testRecruit6.Dialogue.Add("refuse_test", new DialogueNode()
            {
                Text = "Aldric's expression grows cold. \"Then you lack the patience required for true leadership. Do not waste more of my time.\"<br><br>He turns away dismissively.",
                Choices = { }
            });

            testRecruit6.Dialogue.Add("insulted", new DialogueNode()
            {
                Text = "Aldric's eyes narrow dangerously, and power surges around him. \"Arrogance AND ignorance. A lethal combination.\" His voice drops to a deadly whisper. \"Let me educate you, child!\"<br><br>Lightning crackles along his staff.",
                Action = new DialogueAction { Type = "trigger_combat" },
                Choices = { }
            });

            // Test Recruit 7 - Legionnaire, Female, Conversational
            NPC testRecruit7 = new NPC();
            testRecruit7.Name = "Thora";
            testRecruit7.Description = "A shield-maiden practices defensive stances.";
            testRecruit7.ShortDescription = "A shield-maiden";
            testRecruit7.IsHostile = false;
            testRecruit7.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "I've been looking for a worthy guild to join. Is this it?",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Join us!", nextNodeID = "recruit_offer" }
                }
            });
            testRecruit7.Dialogue.Add("recruit_offer", new DialogueNode()
            {
                Text = "I'm in. Let's make this guild legendary.",
                Action = new DialogueAction { Type = "add_recruit", Parameters = { { "class", "Legionnaire" } } },
                Choices = { }
            });

            // Test Recruit 8 - Venator, Male, Conversational
            NPC testRecruit8 = new NPC();
            testRecruit8.Name = "Fenris";
            testRecruit8.Description = "A tracker studies animal prints in the dirt, his wolf companion nearby.";
            testRecruit8.ShortDescription = "A beast tracker";
            testRecruit8.IsHostile = false;
            testRecruit8.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "Me and my wolf are available for hire. Guild work sounds good.",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Join the guild?", nextNodeID = "recruit_offer" }
                }
            });
            testRecruit8.Dialogue.Add("recruit_offer", new DialogueNode()
            {
                Text = "We're in. The guild hall is south of the crossroads, right?",
                Action = new DialogueAction { Type = "add_recruit", Parameters = { { "class", "Venator" } } },
                Choices = { }
            });

            // Test Recruit 9 - Oracle, Female, Conversational
            NPC testRecruit9 = new NPC();
            testRecruit9.Name = "Celestia";
            testRecruit9.Description = "A priestess meditates peacefully, surrounded by softly glowing orbs of light.";
            testRecruit9.ShortDescription = "A priestess";
            testRecruit9.IsHostile = false;
            testRecruit9.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "I sense great potential in your guild. I would be honored to join.",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Please do!", nextNodeID = "recruit_offer" }
                }
            });
            testRecruit9.Dialogue.Add("recruit_offer", new DialogueNode()
            {
                Text = "The light guides me to your guild hall. I shall go there now.",
                Action = new DialogueAction { Type = "add_recruit", Parameters = { { "class", "Oracle" } } },
                Choices = { }
            });

            // Add test recruits
            npcs.Add(testRecruit1.Name, testRecruit1);
            npcs.Add(testRecruit2.Name, testRecruit2);
            npcs.Add(testRecruit3.Name, testRecruit3);
            npcs.Add(testRecruit4.Name, testRecruit4);
            npcs.Add(testRecruit5.Name, testRecruit5);
            npcs.Add(testRecruit6.Name, testRecruit6);
            npcs.Add(testRecruit7.Name, testRecruit7);
            npcs.Add(testRecruit8.Name, testRecruit8);
            npcs.Add(testRecruit9.Name, testRecruit9);

            // Senator Quintus - Cipher expert in town hall
            NPC quintus = new NPC();
            quintus.Name = "Senator Quintus";
            quintus.Description = "An older man with dark gray hair, dressed in the white robes and colored sashes marking him as a senator. Despite his age, his eyes are sharp and observant. He works at a desk covered in scrolls and documents.";
            quintus.ShortDescription = "Senator Quintus";
            quintus.IsHostile = false;

            // First greeting - full introduction
            quintus.Dialogue.Add("first_greeting", new DialogueNode()
            {
                Text = "The senator looks up from his work and offers a polite nod. \"Greetings, traveler. I am Senator Quintus. While I serve in the capital city of Aevoria, I am native to Belum and return here often to handle administrative matters for the town.\" He gestures to the documents on his desk. \"How may I assist you today?\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"Senator, I found these documents in a cultist hideout. You should see this.\"",
                        nextNodeID = "hideout_discovered",
                        IsAvailable = (inventory) => (inventory.Contains("cultist orders") || inventory.Contains("ritual notes") || inventory.Contains("philosophical tract"))
                    },
                    new DialogueNode.Choice {
                        choiceText = "\"I found this letter on the Bandit Warlord. Can you decipher it?\"",
                        nextNodeID = "give_letter",
                        IsAvailable = (inventory) => inventory.Contains("indecipherable letter"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "indecipherable letter"} } }
                    },
                    new DialogueNode.Choice { choiceText = "\"Tell me about yourself.\"", nextNodeID = "about_quintus" },
                    new DialogueNode.Choice { choiceText = "\"Just looking around. Thank you.\"", nextNodeID = "end" }
                }
            });

            // Repeat greeting - shorter for subsequent visits
            quintus.Dialogue.Add("repeat_greeting", new DialogueNode()
            {
                Text = "Senator Quintus looks up from his paperwork. \"Ah, back again. How can I help you?\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"Senator, I found these documents in a cultist hideout. You should see this.\"",
                        nextNodeID = "hideout_discovered",
                        IsAvailable = (inventory) => (inventory.Contains("cultist orders") || inventory.Contains("ritual notes") || inventory.Contains("philosophical tract"))
                    },
                    new DialogueNode.Choice {
                        choiceText = "\"I found this letter on the Bandit Warlord. Can you decipher it?\"",
                        nextNodeID = "give_letter",
                        IsAvailable = (inventory) => inventory.Contains("indecipherable letter"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "indecipherable letter"} } }
                    },
                    new DialogueNode.Choice { choiceText = "\"Tell me about yourself.\"", nextNodeID = "about_quintus" },
                    new DialogueNode.Choice { choiceText = "\"Just looking around. Thank you.\"", nextNodeID = "end" }
                }
            });

            quintus.Dialogue.Add("about_quintus", new DialogueNode()
            {
                Text = "Quintus leans back in his chair. \"I've spent most of my career in Aevoria, serving in the Senate. But Belum is my home, and I try to give back when I can.\" A subtle smile crosses his face. \"I've also maintained certain... interests from my younger days. Let's just say I've always had an appreciation for those who venture into danger for the greater good.\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"I found this letter on the Bandit Warlord. Can you decipher it?\"",
                        nextNodeID = "give_letter",
                        IsAvailable = (inventory) => inventory.Contains("indecipherable letter"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "indecipherable letter"} } }
                    },
                    new DialogueNode.Choice { choiceText = "\"I should get going.\"", nextNodeID = "end" }
                }
            });

            quintus.Dialogue.Add("give_letter", new DialogueNode()
            {
                Text = "You hand the letter to Quintus. He opens it and studies it closely for several minutes, his brow furrowing in concentration. Finally, he looks up.<br><br>\"Yes, I recognize this cipher. It's an old one, but I can translate it. This will take some time, though - give me a full day to work on it. Meet me back here at this time tomorrow, and I'll have it ready for you.\"<br><br>He carefully sets the letter aside on his desk.",
                Action = new DialogueAction {
                    Type = "start_timer",
                    Parameters = {
                        {"timer_id", "quintus_translation"},
                        {"duration_hours", "24"}
                    }
                },
                Choices = { }
            });

            // Waiting dialogue - player returns before 24 hours
            quintus.Dialogue.Add("waiting", new DialogueNode()
            {
                Text = "Quintus looks up from his work on the letter. \"I'm still working on the translation. These ciphers take time to crack properly. Come back tomorrow, please.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I'll return later.\"", nextNodeID = "end" }
                }
            });

            // Translation ready - player returns after 24 hours
            quintus.Dialogue.Add("translation_ready", new DialogueNode()
            {
                Text = "Quintus rises as you approach, holding both the original letter and a fresh parchment with his translation. \"Ah, excellent timing. I've finished the translation.\" He hands you the documents.<br><br>\"The letter has two parts - one asking 'How soon can you deliver on your part of the plan?' and the Warlord's unsent response saying he's no longer interested.\"<br><br>He taps the bottom of the page. \"There's a passphrase at the end: 'Ordo Dissolutus.' If someone is using passphrases, they're protecting something valuable.\"",
                Action = new DialogueAction { Type = "receive_item", Parameters = { {"item", "translated letter"} } },
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Do you know who sent the original message?\"", nextNodeID = "about_sender" }
                }
            });

            quintus.Dialogue.Add("about_sender", new DialogueNode()
            {
                Text = "Quintus shakes his head. \"The letter is unsigned, but whoever wrote it was no common criminal. The cipher itself suggests education and resources. Given the context, I'd wager you're dealing with something more organized than simple bandits.\" He pauses thoughtfully. \"Be careful out there. If this passphrase opens something, it may lead you somewhere dangerous.\"<br><br>He strokes his chin. \"If you're looking for answers about where to use this passphrase... you might want to speak with Caelia, the High Priestess at the Temple of Keius. She has knowledge of hidden places and ancient secrets. If anyone would know about secretive groups operating in the area, it would be her.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I'll seek her out. Thank you for your help, Senator.\"", nextNodeID = "end" }
                }
            });

            // Post-hideout dialogue - player returns with cultist documents
            quintus.Dialogue.Add("hideout_discovered", new DialogueNode()
            {
                Text = "Quintus's eyes widen as he examines the documents you've brought. \"By the gods... 'Strike at the heart,' 'remove the pillar,' references to the anniversary festival...\"<br><br>He spreads the papers across his desk, his expression growing darker. \"This is worse than I feared. This isn't just bandits or common criminals. This is an organized cult planning something catastrophic.\"<br><br>He points to symbols at the bottom of one document. \"These markings here - I can't decipher them alone. They appear to be religious in nature, but not from any faith I recognize.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"What do you need to figure this out?\"", nextNodeID = "examination_time" }
                }
            });

            quintus.Dialogue.Add("examination_time", new DialogueNode()
            {
                Text = "Quintus gathers the documents carefully. \"I'll need to consult with High Priestess Caelia at the temple - she's an expert in ancient faiths and forgotten symbols. Together, we should be able to decipher these markings and understand the full scope of this threat.\"<br><br>He looks at you seriously. \"Give us two days to examine everything thoroughly. Meet me back at your guild hall study in 48 hours - I'll bring Caelia and we'll discuss what we've learned. This is too important for rushed work.\"",
                Action = new DialogueAction {
                    Type = "start_timer",
                    Parameters = {
                        {"timer_id", "quintus_examination"},
                        {"duration_hours", "48"}
                    }
                },
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I'll be there. Two days.\"", nextNodeID = "end_examination_start" }
                }
            });

            quintus.Dialogue.Add("end_examination_start", new DialogueNode()
            {
                Text = "\"Good. Use this time to prepare yourself - I have a feeling whatever we uncover will require action.\" He returns to studying the documents, already deep in thought.",
                Choices = { }
            });

            // Waiting dialogue - player returns before 48 hours for examination
            quintus.Dialogue.Add("waiting_examination", new DialogueNode()
            {
                Text = "Quintus is surrounded by scrolls and ancient texts, working alongside several temple scribes. He looks up briefly. \"Still working on these documents. Caelia and I need more time. Meet us at your guild hall study when the two days are up.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I'll return later.\"", nextNodeID = "end" }
                }
            });

            // Examination complete - directs player to guild study
            quintus.Dialogue.Add("examination_complete", new DialogueNode()
            {
                Text = "Quintus looks up with urgency as you approach. \"We've finished our analysis. Caelia and I are heading to your guild hall study now - what we've discovered is... concerning. Meet us there as soon as you can. This can't wait.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"I'll head there immediately.\"", nextNodeID = "end" }
                }
            });

            quintus.Dialogue.Add("end", new DialogueNode()
            {
                Text = "Quintus nods courteously. \"Safe travels, adventurer. My door is always open if you need assistance.\"",
                Choices = { }
            });

            npcs.Add(quintus.Name, quintus);

            // ===== EMPEROR CERTIUS - Ruler of the Empire =====
            NPC emperor = new NPC();
            emperor.Name = "Emperor Certius";
            emperor.Description = "Emperor Certius Rex Maximus stands before you - a man in his sixties with commanding presence despite his age. His silver hair is crowned with a simple golden circlet, and his purple robes bear the imperial eagle. Kind eyes and laugh lines speak to a benevolent ruler, while his bearing shows the confidence of one who has shaped an empire. He exudes warmth and pride, clearly in excellent spirits.";
            emperor.ShortDescription = "Emperor Certius";
            emperor.IsHostile = false;

            // First greeting
            emperor.Dialogue.Add("first_greeting", new DialogueNode()
            {
                Text = "The Emperor rises from his throne as you enter, a warm smile crossing his weathered face. \"Ah, you must be the one Quintus spoke so highly of!\" His voice carries the weight of authority tempered with genuine friendliness. \"The guild master who uncovered this cult business. Come, come - let us speak as equals. Any friend of Quintus is a friend of mine.\"<br><br>He gestures to a chair. \"I must confess, I'm far more excited about the anniversary celebration than worried about some doomsday cult, but Quintus assures me you have urgent information. Before we discuss such grim matters, tell me - what would you like to know about your Emperor?\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Your Majesty, we must discuss the cult threat immediately.\"", nextNodeID = "cult_warning" },
                    new DialogueNode.Choice { choiceText = "\"Tell me about your conquests, Emperor. Your legacy is legendary.\"", nextNodeID = "about_conquests" },
                    new DialogueNode.Choice { choiceText = "\"What are your plans for the Empire's future?\"", nextNodeID = "future_plans" },
                    new DialogueNode.Choice { choiceText = "\"If I may ask, Your Majesty, tell me about yourself personally.\"", nextNodeID = "personal_questions" }
                }
            });

            // Repeat greeting
            emperor.Dialogue.Add("repeat_greeting", new DialogueNode()
            {
                Text = "Emperor Certius greets you warmly. \"Ah, guild master! Back again? I enjoy our conversations. What would you like to discuss?\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Your Majesty, about the cult threat...\"", nextNodeID = "cult_warning" },
                    new DialogueNode.Choice { choiceText = "\"Tell me more about your conquests.\"", nextNodeID = "about_conquests" },
                    new DialogueNode.Choice { choiceText = "\"What are your plans for the future?\"", nextNodeID = "future_plans" },
                    new DialogueNode.Choice { choiceText = "\"I'd like to know more about you personally, if that's appropriate.\"", nextNodeID = "personal_questions" },
                    new DialogueNode.Choice { choiceText = "\"Just paying my respects, Your Majesty.\"", nextNodeID = "end" }
                }
            });

            // About conquests
            emperor.Dialogue.Add("about_conquests", new DialogueNode()
            {
                Text = "Certius's eyes light up with pride. \"Ah, the campaigns! Where to begin?\" He walks to a map on the wall. \"When I took the throne forty years ago, the Empire was half its current size. The northern tribes raided our borders with impunity. The eastern kingdoms refused to acknowledge our sovereignty.\"<br><br>He traces routes on the map. \"The Northern Campaign lasted eight years. I led the Sixth and Eighth Legions personally - young and foolish, my advisors said, but we crushed the tribal alliance at the Battle of Frozen River. Twenty thousand warriors, and we routed them with only four thousand casualties.\"<br><br>\"The eastern kingdoms fell like dominoes after that. They saw our strength and chose diplomacy over destruction. Wise of them.\" He smiles. \"Fifteen kingdoms now pay tribute and enjoy imperial protection. Peace through strength - that's been my philosophy.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Impressive. And the southern expansion?\"", nextNodeID = "southern_campaign" },
                    new DialogueNode.Choice { choiceText = "\"You led legions personally? That's remarkable.\"", nextNodeID = "personal_combat" },
                    new DialogueNode.Choice { choiceText = "\"Thank you for sharing, Your Majesty.\"", nextNodeID = "return_to_greeting" }
                }
            });

            emperor.Dialogue.Add("southern_campaign", new DialogueNode()
            {
                Text = "\"Ah yes, the south!\" Certius chuckles. \"That was... complicated. The Southern Kingdoms were more civilized, you see. Great cities, rich culture, formidable armies. We couldn't simply conquer them - we had to win them over.\"<br><br>\"I spent five years in diplomatic missions, arranging marriages between their nobles and ours, establishing trade agreements, cultural exchanges. My daughter married the Crown Prince of Solaria - creating the strongest alliance in imperial history.\"<br><br>\"Only two kingdoms refused to negotiate. They fell quickly when they realized they stood alone. Now all of the south prospers under the Imperial Peace. Trade has tripled, banditry has been eliminated, and the people are safer than ever.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Diplomacy and strength - a wise combination.\"", nextNodeID = "return_to_greeting" }
                }
            });

            emperor.Dialogue.Add("personal_combat", new DialogueNode()
            {
                Text = "The Emperor grins. \"Oh, in my youth I fancied myself quite the warrior! Trained since childhood in the imperial martial traditions. My father insisted that every emperor should be able to defend the Empire with sword as well as decree.\"<br><br>\"I've scars to prove I wasn't just a figurehead at Frozen River - took an axe to the shoulder that still aches in winter. But I learned my lesson after that. A dead emperor helps no one. I left the frontline fighting to younger, more expendable commanders.\"<br><br>He laughs heartily. \"Though I'll admit, some nights I miss the simplicity of battle. Politics is far more treacherous than any battlefield.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"A warrior-emperor. The people are fortunate.\"", nextNodeID = "return_to_greeting" }
                }
            });

            // Future plans
            emperor.Dialogue.Add("future_plans", new DialogueNode()
            {
                Text = "Certius settles back into his throne, expression thoughtful. \"The Empire is vast, but there's always more to accomplish. My immediate focus is the anniversary celebration - 1500 years of continuous civilization! We're hosting games at the Colosseum, feasts throughout the city, diplomatic envoys from every corner of the known world.\"<br><br>\"Beyond that?\" He leans forward. \"I want to establish the Imperial Academy - a center of learning to rival the ancient schools of philosophy. Knowledge preserved and expanded for future generations.\"<br><br>\"I'm also funding expeditions beyond our borders. There are lands to the far west none have explored. Imagine - expanding human knowledge itself! That would be a legacy worth leaving.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"The Imperial Academy sounds magnificent.\"", nextNodeID = "academy_details" },
                    new DialogueNode.Choice { choiceText = "\"Exploration beyond the known world is ambitious.\"", nextNodeID = "exploration_details" },
                    new DialogueNode.Choice { choiceText = "\"Noble goals, Your Majesty.\"", nextNodeID = "return_to_greeting" }
                }
            });

            emperor.Dialogue.Add("academy_details", new DialogueNode()
            {
                Text = "\"Isn't it though?\" The Emperor's enthusiasm is infectious. \"I've already commissioned the building - it will be the largest library in the world. Scholars from every discipline will teach there: mathematics, philosophy, natural sciences, engineering, medicine.\"<br><br>\"Free admission for any citizen with the aptitude, regardless of birth. Can you imagine? A farmer's son could become the Empire's greatest mathematician! That's true progress.\"<br><br>His eyes shine with excitement. \"Construction begins after the anniversary celebration. It's my gift to future generations - a foundation of knowledge that will outlast any military conquest.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"A lasting legacy indeed.\"", nextNodeID = "return_to_greeting" }
                }
            });

            emperor.Dialogue.Add("exploration_details", new DialogueNode()
            {
                Text = "\"My cartographers say there are entire continents we've never mapped!\" Certius gestures excitedly. \"The western ocean may lead to new lands, new peoples, new knowledge. I've funded three expeditions already - two returned with fascinating discoveries, one is still at sea.\"<br><br>\"Think of it - what if there's an empire equal to ours across that ocean? What could we learn from them? What could we teach them? The possibilities are endless!\"<br><br>He chuckles. \"My advisors think I'm mad, spending the treasury on ships that might never return. But what's the point of empire if not to push the boundaries of what's possible?\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"An inspiring vision, Your Majesty.\"", nextNodeID = "return_to_greeting" }
                }
            });

            // Personal questions
            emperor.Dialogue.Add("personal_questions", new DialogueNode()
            {
                Text = "Certius laughs warmly. \"Most people are too intimidated to ask! I like your boldness. What would you like to know? I'm an open book - well, mostly.\"<br><br>\"I'm sixty-two years old, married to Empress Livia for forty-three wonderful years. Three children: my son Marcus commands the Tenth Legion in the north, my daughter Julia is married to the King of Solaria, and my youngest, Titus, studies philosophy and drives me mad with his abstract questions.\"<br><br>\"I wake at dawn every day, review reports from across the Empire over breakfast, hold court until midday, then spend afternoons on whatever project captures my interest. Simple pleasures - good wine, interesting conversation, watching the sunset from the palace gardens.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"You sound like a happy man, Your Majesty.\"", nextNodeID = "happy_emperor" },
                    new DialogueNode.Choice { choiceText = "\"Tell me about the Empress.\"", nextNodeID = "about_empress" },
                    new DialogueNode.Choice { choiceText = "\"Thank you for your candor.\"", nextNodeID = "return_to_greeting" }
                }
            });

            emperor.Dialogue.Add("happy_emperor", new DialogueNode()
            {
                Text = "\"I am!\" Certius beams. \"Not every emperor can say that. The crown is a burden, certainly, but it's also a privilege. I've seen the Empire prosper, watched millions of people live better lives because of decisions I've made.\"<br><br>\"Yes, there are always problems - there always will be. But look at what we've built! Fifteen hundred years of continuous civilization. Roads connecting every corner of the empire. Cities where art and knowledge flourish. Common people living in peace and prosperity.\"<br><br>He spreads his hands. \"How could I not be happy? I'm the luckiest man alive.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"The Empire is fortunate to have you.\"", nextNodeID = "return_to_greeting" }
                }
            });

            emperor.Dialogue.Add("about_empress", new DialogueNode()
            {
                Text = "The Emperor's expression softens. \"Aurelia is the steel behind the throne. Where I'm all grand visions and bold declarations, she's practical wisdom and careful planning. We balance each other perfectly.\"<br><br>\"We met when I was crown prince - she was daughter of a general, brilliant and beautiful. My father approved the match for political reasons, but I fell genuinely in love. Forty-three years later, I still do.\"<br><br>\"She'll join us for the anniversary celebration. You'll meet her - though I warn you, she's far more skeptical of your cult warnings than I am. She thinks we're both being paranoid.\" He chuckles. \"She's probably right.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"A true partnership. That's wonderful.\"", nextNodeID = "return_to_greeting" }
                }
            });

            // Cult warning (main quest)
            emperor.Dialogue.Add("cult_warning", new DialogueNode()
            {
                Text = "Certius's jovial expression becomes more serious, though you can tell he's humoring you. \"Yes, yes - Quintus briefed me on your discoveries. A cult planning to assassinate me during the celebration, break some ancient seals, end the world. Quite dramatic!\"<br><br>He leans back. \"Look, I appreciate your concern. Truly. And I'll have the Imperial Guard increase security during the games. But...\" He smiles gently. \"I've been emperor for forty years. There's always someone plotting assassination. The guard is the best in the world - they'll handle it.\"<br><br>\"More importantly, I can't cancel or modify the celebration. Fifteen hundred years! Diplomatic envoys from every kingdom will be there. To show fear would be to show weakness. The Empire doesn't cower before threats.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Your Majesty, the Five Seals are real. This is apocalyptic.\"", nextNodeID = "cult_insistence" },
                    new DialogueNode.Choice { choiceText = "\"I understand your position, but please be careful.\"", nextNodeID = "cult_acceptance" }
                }
            });

            emperor.Dialogue.Add("cult_insistence", new DialogueNode()
            {
                Text = "Certius regards you kindly but skeptically. \"The Five Seals are mythology, friend. Old legends from before the Empire. I respect that you believe otherwise - and I respect Caelia's scholarship on ancient faiths - but I cannot make decisions based on myths.\"<br><br>He stands, placing a hand on your shoulder. \"I tell you what - I'll let you attend the celebration. You and your party can watch for these cultists yourselves. Consider yourselves... unofficial imperial guards for the day. If you spot a genuine threat, raise the alarm. Fair enough?\"",
                Action = new DialogueAction { Type = "set_quest_flag", Parameters = { {"flag", "emperor_warned"}, {"value", "true"} } },
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Thank you, Your Majesty. We'll be vigilant.\"", nextNodeID = "return_to_greeting" }
                }
            });

            emperor.Dialogue.Add("cult_acceptance", new DialogueNode()
            {
                Text = "\"I will be careful, you have my word.\" Certius nods appreciatively. \"And I'm grateful you made the journey to warn me. That shows character.\"<br><br>He considers for a moment. \"Tell you what - attend the celebration as my guests. You can keep watch for these cultists, and if you see anything suspicious, you'll have direct access to me. How's that?\"",
                Action = new DialogueAction { Type = "set_quest_flag", Parameters = { {"flag", "emperor_warned"}, {"value", "true"} } },
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"We'd be honored, Your Majesty.\"", nextNodeID = "return_to_greeting" }
                }
            });

            // Return to greeting options
            emperor.Dialogue.Add("return_to_greeting", new DialogueNode()
            {
                Text = "\"Was there anything else you wished to discuss?\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Tell me more about your conquests.\"", nextNodeID = "about_conquests" },
                    new DialogueNode.Choice { choiceText = "\"What are your plans for the future?\"", nextNodeID = "future_plans" },
                    new DialogueNode.Choice { choiceText = "\"I'd like to know more about you personally.\"", nextNodeID = "personal_questions" },
                    new DialogueNode.Choice { choiceText = "\"That's all for now, Your Majesty.\"", nextNodeID = "end" }
                }
            });

            emperor.Dialogue.Add("end", new DialogueNode()
            {
                Text = "\"Excellent conversation! Feel free to explore the villa - the gardens are particularly lovely this time of year. I'll see you at the celebration!\"",
                Choices = { }
            });

            npcs.Add(emperor.Name, emperor);

            // Caelia - High Priestess of Keius at temple
            // ===== CULTIST ENEMIES - Forest Hideout (Level 6-8) =====

            // Cultist Scout - Light, fast, weak
            NPC cultistScout = new NPC();
            cultistScout.Name = "Cultist Scout";
            cultistScout.Description = "A lightly-armored figure in tattered robes, moving with nervous energy. Their eyes dart constantly, watching for intruders.";
            cultistScout.ShortDescription = "A cultist scout";
            cultistScout.IsHostile = true;
            cultistScout.Health = 40;
            cultistScout.MaxHealth = 40;
            cultistScout.Energy = 20;
            cultistScout.MaxEnergy = 20;
            cultistScout.AttackDamage = 8;
            cultistScout.Defense = 3;
            cultistScout.Speed = 14;
            cultistScout.DamageCount = 1;
            cultistScout.DamageDie = 8;
            cultistScout.DamageBonus = 6;
            cultistScout.MinGold = 10;
            cultistScout.MaxGold = 25;
            cultistScout.ExperienceReward = 100;
            cultistScout.Role = EnemyRole.Melee;
            cultistScout.LootTable = new Dictionary<string, int> { {"potion", 40}, {"energy potion", 30}, {"dagger", 15} };

            // Cultist Zealot - Medium, basic magic user
            NPC cultistZealot = new NPC();
            cultistZealot.Name = "Cultist Zealot";
            cultistZealot.Description = "A robed figure clutching a profane symbol, eyes burning with fanatic devotion. Dark energy crackles around their fingertips.";
            cultistZealot.ShortDescription = "A cultist zealot";
            cultistZealot.IsHostile = true;
            cultistZealot.Health = 55;
            cultistZealot.MaxHealth = 55;
            cultistZealot.Energy = 35;
            cultistZealot.MaxEnergy = 35;
            cultistZealot.AttackDamage = 10;
            cultistZealot.Defense = 4;
            cultistZealot.Speed = 10;
            cultistZealot.DamageCount = 1;
            cultistZealot.DamageDie = 10;
            cultistZealot.DamageBonus = 8;
            cultistZealot.MinGold = 15;
            cultistZealot.MaxGold = 30;
            cultistZealot.ExperienceReward = 120;
            cultistZealot.Role = EnemyRole.Ranged;
            cultistZealot.AbilityNames = new List<string> { "Entropy Bolt", "Void Touch" };
            cultistZealot.LootTable = new Dictionary<string, int> { {"potion", 50}, {"energy potion", 60}, {"scroll of fireball", 20} };

            // Cultist Defacer - Equipment damage abilities
            NPC cultistDefacer = new NPC();
            cultistDefacer.Name = "Cultist Defacer";
            cultistDefacer.Description = "A wild-eyed cultist carrying tools of destruction - hammers, chisels, and acid vials. They exist to unmake and destroy.";
            cultistDefacer.ShortDescription = "A cultist defacer";
            cultistDefacer.IsHostile = true;
            cultistDefacer.Health = 60;
            cultistDefacer.MaxHealth = 60;
            cultistDefacer.Energy = 25;
            cultistDefacer.MaxEnergy = 25;
            cultistDefacer.AttackDamage = 11;
            cultistDefacer.Defense = 5;
            cultistDefacer.Speed = 9;
            cultistDefacer.DamageCount = 1;
            cultistDefacer.DamageDie = 12;
            cultistDefacer.DamageBonus = 9;
            cultistDefacer.MinGold = 12;
            cultistDefacer.MaxGold = 28;
            cultistDefacer.ExperienceReward = 130;
            cultistDefacer.Role = EnemyRole.Melee;
            cultistDefacer.AbilityNames = new List<string> { "Corrosive Strike" };
            cultistDefacer.LootTable = new Dictionary<string, int> { {"potion", 45}, {"antidote", 50} };

            // Cultist Philosopher - Support, buffs other cultists
            NPC cultistPhilosopher = new NPC();
            cultistPhilosopher.Name = "Cultist Philosopher";
            cultistPhilosopher.Description = "An older cultist in pristine robes, speaking in measured tones about entropy and decay. Their calm demeanor is unnerving.";
            cultistPhilosopher.ShortDescription = "A cultist philosopher";
            cultistPhilosopher.IsHostile = true;
            cultistPhilosopher.Health = 50;
            cultistPhilosopher.MaxHealth = 50;
            cultistPhilosopher.Energy = 40;
            cultistPhilosopher.MaxEnergy = 40;
            cultistPhilosopher.AttackDamage = 7;
            cultistPhilosopher.Defense = 6;
            cultistPhilosopher.Speed = 8;
            cultistPhilosopher.DamageCount = 1;
            cultistPhilosopher.DamageDie = 6;
            cultistPhilosopher.DamageBonus = 5;
            cultistPhilosopher.MinGold = 20;
            cultistPhilosopher.MaxGold = 40;
            cultistPhilosopher.ExperienceReward = 140;
            cultistPhilosopher.Role = EnemyRole.Support;
            cultistPhilosopher.AbilityNames = new List<string> { "Defensive Stance" };
            cultistPhilosopher.LootTable = new Dictionary<string, int> { {"energy potion", 70}, {"greater potion", 40}, {"scroll of protection", 25} };

            // Cultist Archivist - Knowledge-themed attacks
            NPC cultistArchivist = new NPC();
            cultistArchivist.Name = "Cultist Archivist";
            cultistArchivist.Description = "A robed figure surrounded by torn pages and ash. They carry burning torches and seem to delight in destroying written knowledge.";
            cultistArchivist.ShortDescription = "A cultist archivist";
            cultistArchivist.IsHostile = true;
            cultistArchivist.Health = 52;
            cultistArchivist.MaxHealth = 52;
            cultistArchivist.Energy = 30;
            cultistArchivist.MaxEnergy = 30;
            cultistArchivist.AttackDamage = 9;
            cultistArchivist.Defense = 4;
            cultistArchivist.Speed = 11;
            cultistArchivist.DamageCount = 2;
            cultistArchivist.DamageDie = 6;
            cultistArchivist.DamageBonus = 7;
            cultistArchivist.MinGold = 15;
            cultistArchivist.MaxGold = 35;
            cultistArchivist.ExperienceReward = 125;
            cultistArchivist.Role = EnemyRole.Ranged;
            cultistArchivist.AbilityNames = new List<string> { "Flame Bolt" };
            cultistArchivist.LootTable = new Dictionary<string, int> { {"potion", 50}, {"energy potion", 55}, {"scroll of fireball", 30} };

            // Cultist Breaker - High damage to structures/armor
            NPC cultistBreaker = new NPC();
            cultistBreaker.Name = "Cultist Breaker";
            cultistBreaker.Description = "A hulking cultist wielding a massive sledgehammer. Their robes are torn, revealing scarred muscles. They live to destroy.";
            cultistBreaker.ShortDescription = "A cultist breaker";
            cultistBreaker.IsHostile = true;
            cultistBreaker.Health = 70;
            cultistBreaker.MaxHealth = 70;
            cultistBreaker.Energy = 20;
            cultistBreaker.MaxEnergy = 20;
            cultistBreaker.AttackDamage = 14;
            cultistBreaker.Defense = 6;
            cultistBreaker.Speed = 7;
            cultistBreaker.DamageCount = 1;
            cultistBreaker.DamageDie = 14;
            cultistBreaker.DamageBonus = 12;
            cultistBreaker.MinGold = 18;
            cultistBreaker.MaxGold = 35;
            cultistBreaker.ExperienceReward = 145;
            cultistBreaker.Role = EnemyRole.Melee;
            cultistBreaker.AbilityNames = new List<string> { "Crushing Blow" };
            cultistBreaker.LootTable = new Dictionary<string, int> { {"potion", 60}, {"greater potion", 35}, {"battle axe", 25} };

            // Cultist Lieutenant - Mini-boss with combination abilities
            NPC cultistLieutenant = new NPC();
            cultistLieutenant.Name = "Cultist Lieutenant";
            cultistLieutenant.Description = "A scarred veteran cultist in ornate defaced robes, wielding both blade and dark magic. Their eyes hold the certainty of fanaticism.";
            cultistLieutenant.ShortDescription = "A cultist lieutenant";
            cultistLieutenant.IsHostile = true;
            cultistLieutenant.Health = 75;
            cultistLieutenant.MaxHealth = 75;
            cultistLieutenant.Energy = 40;
            cultistLieutenant.MaxEnergy = 40;
            cultistLieutenant.AttackDamage = 12;
            cultistLieutenant.Defense = 7;
            cultistLieutenant.Speed = 11;
            cultistLieutenant.DamageCount = 2;
            cultistLieutenant.DamageDie = 8;
            cultistLieutenant.DamageBonus = 10;
            cultistLieutenant.MinGold = 25;
            cultistLieutenant.MaxGold = 50;
            cultistLieutenant.ExperienceReward = 160;
            cultistLieutenant.Role = EnemyRole.Melee;
            cultistLieutenant.AbilityNames = new List<string> { "Entropy Bolt", "Crushing Blow" };
            cultistLieutenant.LootTable = new Dictionary<string, int> { {"greater potion", 80}, {"greater energy potion", 60}, {"steel gladius", 40}, {"scroll of haste", 25} };

            // Cultist Unraveler - Reality-bending magic user
            NPC cultistUnraveler = new NPC();
            cultistUnraveler.Name = "Cultist Unraveler";
            cultistUnraveler.Description = "A hunched figure whose very presence seems to distort the air. Reality ripples around them as they chant words that unmake.";
            cultistUnraveler.ShortDescription = "A cultist unraveler";
            cultistUnraveler.IsHostile = true;
            cultistUnraveler.Health = 48;
            cultistUnraveler.MaxHealth = 48;
            cultistUnraveler.Energy = 45;
            cultistUnraveler.MaxEnergy = 45;
            cultistUnraveler.AttackDamage = 11;
            cultistUnraveler.Defense = 3;
            cultistUnraveler.Speed = 12;
            cultistUnraveler.DamageCount = 2;
            cultistUnraveler.DamageDie = 8;
            cultistUnraveler.DamageBonus = 8;
            cultistUnraveler.MinGold = 20;
            cultistUnraveler.MaxGold = 45;
            cultistUnraveler.ExperienceReward = 150;
            cultistUnraveler.Role = EnemyRole.Ranged;
            cultistUnraveler.AbilityNames = new List<string> { "Void Touch", "Entropy Bolt", "Flame Bolt" };
            cultistUnraveler.LootTable = new Dictionary<string, int> { {"greater energy potion", 80}, {"elixir of vigor", 30}, {"scroll of fireball", 35}, {"scroll of healing", 25} };

            // Archon Malachar - Cultist Leader Boss (Level 8)
            NPC archonMalachar = new NPC();
            archonMalachar.Name = "Archon Malachar";
            archonMalachar.Description = "The cult leader stands before an altar of defaced idols, radiating dark purpose. His robes are covered in symbols of entropy and decay. In his hand, a ritual dagger pulses with unnatural energy.";
            archonMalachar.ShortDescription = "Archon Malachar";
            archonMalachar.PreCombatDialogue = "Archon Malachar turns from the altar, his eyes burning with fanatic certainty. \"You've come far, but you understand nothing. The Empire built its glory on stolen foundations - ancient seals that bind what should be free. We will undo their theft. We will return everything to its natural state.\" He raises the ritual dagger. \"Your interference ends here.\"";
            archonMalachar.IsHostile = true;
            archonMalachar.Health = 95;
            archonMalachar.MaxHealth = 95;
            archonMalachar.Energy = 50;
            archonMalachar.MaxEnergy = 50;
            archonMalachar.AttackDamage = 13;
            archonMalachar.Defense = 8;
            archonMalachar.Speed = 13;
            archonMalachar.DamageCount = 2;
            archonMalachar.DamageDie = 10;
            archonMalachar.DamageBonus = 11;
            archonMalachar.MinGold = 50;
            archonMalachar.MaxGold = 100;
            archonMalachar.ExperienceReward = 250;
            archonMalachar.Role = EnemyRole.Ranged;
            archonMalachar.AbilityNames = new List<string> { "Entropy Bolt", "Void Touch", "Flame Bolt" };
            archonMalachar.LootTable = new Dictionary<string, int>
            {
                {"ritual dagger", 100},  // Quest item
                {"cultist orders", 100},  // Quest document
                {"greater potion", 100},
                {"greater energy potion", 80},
                {"elixir of vigor", 50},
                {"scroll of haste", 40},
                {"scroll of protection", 35}
            };

            // Imperial Assassin - Boss (Level 9) - Emperor's killer
            NPC imperialAssassin = new NPC();
            imperialAssassin.Name = "Imperial Assassin";
            imperialAssassin.Description = "The assassin has discarded their imperial guard helmet, revealing a face covered in cultist tattoos. Their eyes burn with fanatic zeal. Blood - the Emperor's blood - still drips from the ornate dagger in their hand. This is a true believer, willing to die for the Unbound's cause.";
            imperialAssassin.ShortDescription = "The Imperial Assassin";
            imperialAssassin.PreCombatDialogue = "The assassin laughs, a sound of pure madness. \"Feel it? The first crack in the seal! Ten thousand witnesses saw him fall! Their terror, their despair - it feeds the breach!\" Blood-flecked spittle flies from their lips. \"You think killing me matters? The work is done! The Empire's pillar has fallen! Soon the others will follow, and the Bound Ones will reshape this broken world!\"";
            imperialAssassin.IsHostile = true;
            imperialAssassin.Health = 110;
            imperialAssassin.MaxHealth = 110;
            imperialAssassin.Energy = 55;
            imperialAssassin.MaxEnergy = 55;
            imperialAssassin.AttackDamage = 15;
            imperialAssassin.Defense = 9;
            imperialAssassin.Speed = 16;
            imperialAssassin.DamageCount = 2;
            imperialAssassin.DamageDie = 12;
            imperialAssassin.DamageBonus = 13;
            imperialAssassin.MinGold = 75;
            imperialAssassin.MaxGold = 150;
            imperialAssassin.ExperienceReward = 350;
            imperialAssassin.Role = EnemyRole.Melee;
            imperialAssassin.AbilityNames = new List<string> { "Void Touch", "Corrosive Strike", "Backstab", "Entropy Bolt" };
            imperialAssassin.LootTable = new Dictionary<string, int>
            {
                {"emperor's blood dagger", 100},  // Quest item - the assassination weapon
                {"greater potion", 100},
                {"greater energy potion", 100},
                {"elixir of vigor", 80},
                {"scroll of haste", 60},
                {"scroll of protection", 50}
            };

            npcs.Add(cultistScout.Name, cultistScout);
            npcs.Add(cultistZealot.Name, cultistZealot);
            npcs.Add(cultistDefacer.Name, cultistDefacer);
            npcs.Add(cultistPhilosopher.Name, cultistPhilosopher);
            npcs.Add(cultistArchivist.Name, cultistArchivist);
            npcs.Add(cultistBreaker.Name, cultistBreaker);
            npcs.Add(cultistLieutenant.Name, cultistLieutenant);
            npcs.Add(cultistUnraveler.Name, cultistUnraveler);
            npcs.Add(archonMalachar.Name, archonMalachar);
            npcs.Add(imperialAssassin.Name, imperialAssassin);

            // Althea - Oracle recruit found in cultist prison (Room 115)
            NPC althea = new NPC();
            althea.Name = "Althea";
            althea.Description = "A young woman in tattered oracle robes huddles in the corner of the cell. Despite obvious signs of captivity - gaunt cheeks, bruises, dirt-streaked face - her eyes still glow with an eerie inner light. She watches you with a mixture of hope and wariness, as if she's seen too many visions of betrayal to trust easily. Silver hair falls in tangles around her face, and strange rune-tattoos shimmer faintly on her forearms.";
            althea.ShortDescription = "An imprisoned oracle";
            althea.IsHostile = false;
            althea.Health = 40;
            althea.MaxHealth = 40;
            althea.Energy = 50;
            althea.MaxEnergy = 50;
            althea.AttackDamage = 8;
            althea.Defense = 3;
            althea.Speed = 10;

            // Althea dialogue - First meeting (imprisoned)
            althea.Dialogue.Add("first_greeting", new DialogueNode()
            {
                Text = "The woman in the cell rises slowly, gripping the bars for support. Her eyes - glowing with an otherworldly light - study you intensely.<br><br>\"You... you're not one of them,\" she says, her voice hoarse from disuse. \"I didn't foresee this. My visions showed only darkness and spirals... but you're here. How?\"<br><br>She seems to catch herself, taking a shaky breath. \"Forgive me. I am Althea. An oracle... or I was, before they took me.\"",
                PartyInterjections = new Dictionary<string, string>
                {
                    { "Braxus", "Braxus freezes, his face going pale. \"Althea...\" His voice cracks. \"By the gods, I found you.\" He grips the cell bars beside you, knuckles white. \"I got your letter. I've been searching for weeks. I thought... I thought you were dead.\"" }
                },
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Who took you? The cultists?\"", nextNodeID = "ask_about_cultists" },
                    new DialogueNode.Choice { choiceText = "\"Are you alright? How long have you been here?\"", nextNodeID = "ask_about_condition" }
                }
            });

            // Subsequent meetings (still imprisoned, before recruitment)
            althea.Dialogue.Add("repeat_greeting", new DialogueNode()
            {
                Text = "Althea looks up as you approach the cell, a weak smile crossing her face. \"You came back. I... I wasn't sure you would.\"",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "\"Tell me more about what the cultists are planning.\"",
                        nextNodeID = "explain_cult_plan",
                        RequireNotDiscussedNode = "explain_cult_plan"
                    },
                    new DialogueNode.Choice {
                        choiceText = "\"I have the cell key. Let me get you out of here.\"",
                        nextNodeID = "offer_recruitment",
                        IsAvailable = (inventory) => inventory.Contains("cell key")
                    },
                    new DialogueNode.Choice { choiceText = "\"I need to go for now.\"", nextNodeID = "end" }
                }
            });

            althea.Dialogue.Add("ask_about_cultists", new DialogueNode()
            {
                Text = "Althea's expression darkens. \"The Ordo Dissolutus. The Dissolved Order. They call themselves philosophers, but they're fanatics. They worship entropy, decay, the unmaking of all things.\"<br><br>She grips the bars tighter. \"They took me because of my visions. They wanted to know about the seals - the ancient barriers that hold back chaos. When I wouldn't help them... they locked me here.\"",
                PartyInterjections = new Dictionary<string, string>
                {
                    { "Braxus", "Althea's eyes find Braxus through the bars, and tears well up. \"I'm so sorry I left. I tried to protect you by disappearing, but...\" Her voice breaks. \"I should have trusted you. You always said we'd face danger together.\"" }
                },
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"What are they planning?\"", nextNodeID = "explain_cult_plan" }
                }
            });

            althea.Dialogue.Add("ask_about_condition", new DialogueNode()
            {
                Text = "Althea looks down at her tattered robes and bruised arms. \"I... I don't know how long it's been. Days? Weeks? Time blurs when you're locked in darkness.\"<br><br>\"They fed me enough to keep me alive. Questioned me endlessly about the seals, about my visions. But I wouldn't tell them what they wanted to know.\"<br><br>Her glowing eyes meet yours. \"Thank you for asking. It's been... a very long time since anyone showed me kindness.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"What were they questioning you about?\"", nextNodeID = "ask_about_cultists" }
                }
            });

            althea.Dialogue.Add("explain_cult_plan", new DialogueNode()
            {
                Text = "Althea's eyes unfocus slightly, as if seeing something far away. \"They're planning something during the anniversary festival in Aevoria. When the empire celebrates, when crowds gather at the capital...\"<br><br>\"In my visions, I saw fire and screaming. The cult intends to strike at 'the pillar' - they speak in metaphors, but I believe they mean someone important. Perhaps... perhaps the Emperor himself.\"<br><br>She shudders. \"If they weaken the seals at the same time, the chaos released by so many deaths could cascade. The barriers could fail. Ancient things that were bound long ago... they could break free.\"",
                PartyInterjections = new Dictionary<string, string>
                {
                    { "Braxus", "Braxus steps closer to the bars, his voice urgent. \"An assassination attempt on the Emperor? Althea, we can stop this together - like we should have done from the start.\" He looks at you. \"Boss, we need to get her out of here. Then we warn Aevoria. We'll need proof, but if anyone can help us, it's her.\"" }
                },
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"We need to warn the Emperor. But first, let me get you out of here.\"", nextNodeID = "offer_rescue" }
                }
            });

            althea.Dialogue.Add("offer_rescue", new DialogueNode()
            {
                Text = "Althea's eyes widen. \"You... you'd do that? You don't even know me.\"<br><br>She pauses, and for a moment her eyes glow brighter. \"No... wait. I see it now. You're the guild master. The one trying to rebuild what was lost.\"<br><br>A genuine smile crosses her face. \"Perhaps the visions knew you were coming after all.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Join my guild. Help me stop this cult and protect the empire.\"", nextNodeID = "offer_recruitment" }
                }
            });

            althea.Dialogue.Add("offer_recruitment", new DialogueNode()
            {
                Text = "You unlock the cell door with the iron key, and it swings open with a rusty creak. Althea steps out hesitantly, as if unsure her legs will hold her weight.<br><br>She straightens despite her weakened state, determination replacing the fear in her eyes. \"Yes. I will join you. My visions may be clouded, but I can see that our paths are meant to cross. Together, we can stop the Ordo Dissolutus and warn Aevoria.\"",
                Action = new DialogueAction {
                    Type = "give_item",
                    Parameters = { { "item", "cell key" } }
                },
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Welcome to the guild.\"", nextNodeID = "recruit_althea" }
                }
            });

            althea.Dialogue.Add("recruit_althea", new DialogueNode()
            {
                Text = "Althea takes a deep breath of free air, her eyes glowing brighter. \"Thank you. I won't forget this. Together, we'll stop the cult and save the Empire.\"",
                Action = new DialogueAction {
                    Type = "add_recruit",
                    Parameters = { { "class", "Oracle" } }
                },
                Choices = new List<DialogueNode.Choice>()
            });

            althea.Dialogue.Add("end", new DialogueNode()
            {
                Text = "Althea nods gratefully. \"Thank you. Be careful - the cultists are dangerous, and their leader is even more so.\"",
                Choices = { } // Conversation ends
            });

            // Guild hall dialogue (after recruitment) - Add simple interaction for now
            althea.Dialogue.Add("guild_hall_greeting", new DialogueNode()
            {
                Text = "Althea looks up from her meditation, her eyes glowing softly. \"Ah, Guild Master. The visions have been clearer since joining you. It's... comforting to have purpose again.\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"How are you settling in?\"", nextNodeID = "guild_settling" },
                    new DialogueNode.Choice { choiceText = "\"I'll leave you to your meditation.\"", nextNodeID = "guild_end" }
                }
            });

            althea.Dialogue.Add("guild_settling", new DialogueNode()
            {
                Text = "Althea smiles faintly. \"Better than I expected. The guild feels... safe. Though I must admit, some of the others are... intense.\"<br><br>She glances across the hall. \"Braxus, for instance. He's still suspicious of me, I think. But his caution is understandable. Perhaps in time, he'll see I mean no harm.\"",
                PartyInterjections = new Dictionary<string, string>
                {
                    { "Braxus", "From across the hall, Braxus grunts. \"I can hear you, oracle. And yeah, I'm keeping an eye on you. Nothing personal - just don't trust magic I can't punch.\" He pauses. \"But... you fought well in that last battle. Keep that up and we'll get along fine.\"" }
                },
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"You'll both fit in here just fine.\"", nextNodeID = "guild_end" }
                }
            });

            althea.Dialogue.Add("guild_end", new DialogueNode()
            {
                Text = "Althea nods and returns to her meditation, a peaceful expression on her face.",
                Choices = { }
            });

            npcs.Add(althea.Name, althea);

            // ===== MYTHOLOGICAL ENEMIES FOR DUNGEON TEST AREA =====

            // Floor 1 Enemies (Level 1-5)
            NPC satyr = new NPC();
            satyr.Name = "Satyr";
            satyr.Description = "A wild creature with goat legs and curved horns, wielding a crude wooden club.";
            satyr.ShortDescription = "A hostile satyr";
            satyr.IsHostile = true;
            satyr.Health = 10;
            satyr.MaxHealth = 10;
            satyr.Energy = 5;
            satyr.MaxEnergy = 5;
            satyr.AttackDamage = 3;
            satyr.DamageCount = 1;
            satyr.DamageDie = 4;
            satyr.DamageBonus = 1;
            satyr.Defense = 0;
            satyr.Speed = 3;
            satyr.IsBackRow = false;
            satyr.MinGold = 25;
            satyr.MaxGold = 35;
            satyr.ExperienceReward = 5;
            satyr.Role = EnemyRole.Melee;
            satyr.LootTable = new Dictionary<string, int> { {"potion", 20} };
            npcs.Add(satyr.Name, satyr);

            NPC harpy = new NPC();
            harpy.Name = "Harpy";
            harpy.Description = "A screeching winged creature with the body of a bird and the face of a woman.";
            harpy.ShortDescription = "A hostile harpy";
            harpy.IsHostile = true;
            harpy.Health = 12;
            harpy.MaxHealth = 12;
            harpy.Energy = 8;
            harpy.MaxEnergy = 8;
            harpy.AttackDamage = 4;
            harpy.DamageCount = 1;
            harpy.DamageDie = 4;
            harpy.DamageBonus = 2;
            harpy.Defense = 1;
            harpy.Speed = 5;
            harpy.IsBackRow = true;
            harpy.MinGold = 30;
            harpy.MaxGold = 40;
            harpy.ExperienceReward = 6;
            harpy.Role = EnemyRole.Ranged;
            harpy.EnergyRegenPerTurn = 2;
            harpy.LootTable = new Dictionary<string, int> { {"potion", 25} };
            npcs.Add(harpy.Name, harpy);

            NPC giantScorpion = new NPC();
            giantScorpion.Name = "Giant Scorpion";
            giantScorpion.Description = "A massive scorpion with gleaming black armor and a venomous stinger.";
            giantScorpion.ShortDescription = "A hostile giant scorpion";
            giantScorpion.IsHostile = true;
            giantScorpion.Health = 15;
            giantScorpion.MaxHealth = 15;
            giantScorpion.Energy = 0;
            giantScorpion.MaxEnergy = 0;
            giantScorpion.AttackDamage = 4;
            giantScorpion.DamageCount = 1;
            giantScorpion.DamageDie = 4;
            giantScorpion.DamageBonus = 2;
            giantScorpion.Defense = 2;
            giantScorpion.Speed = 2;
            giantScorpion.IsBackRow = false;
            giantScorpion.MinGold = 35;
            giantScorpion.MaxGold = 45;
            giantScorpion.ExperienceReward = 7;
            giantScorpion.Role = EnemyRole.Melee;
            giantScorpion.LootTable = new Dictionary<string, int> { {"potion", 30} };
            npcs.Add(giantScorpion.Name, giantScorpion);

            NPC skeletonWarrior = new NPC();
            skeletonWarrior.Name = "Skeleton Warrior";
            skeletonWarrior.Description = "An animated skeleton wielding a rusty sword and shield.";
            skeletonWarrior.ShortDescription = "A hostile skeleton warrior";
            skeletonWarrior.IsHostile = true;
            skeletonWarrior.Health = 14;
            skeletonWarrior.MaxHealth = 14;
            skeletonWarrior.Energy = 0;
            skeletonWarrior.MaxEnergy = 0;
            skeletonWarrior.AttackDamage = 4;
            skeletonWarrior.DamageCount = 1;
            skeletonWarrior.DamageDie = 4;
            skeletonWarrior.DamageBonus = 2;
            skeletonWarrior.Defense = 1;
            skeletonWarrior.Speed = 2;
            skeletonWarrior.IsBackRow = false;
            skeletonWarrior.MinGold = 30;
            skeletonWarrior.MaxGold = 40;
            skeletonWarrior.ExperienceReward = 5;
            skeletonWarrior.Role = EnemyRole.Melee;
            skeletonWarrior.LootTable = new Dictionary<string, int> { {"potion", 25} };
            npcs.Add(skeletonWarrior.Name, skeletonWarrior);

            // Floor 2 Enemies (Level 6-10)
            NPC centaurScout = new NPC();
            centaurScout.Name = "Centaur Scout";
            centaurScout.Description = "A proud centaur warrior with bow drawn, hooves pawing the ground.";
            centaurScout.ShortDescription = "A hostile centaur scout";
            centaurScout.IsHostile = true;
            centaurScout.Health = 45;
            centaurScout.MaxHealth = 45;
            centaurScout.Energy = 20;
            centaurScout.MaxEnergy = 20;
            centaurScout.AttackDamage = 8;
            centaurScout.DamageCount = 1;
            centaurScout.DamageDie = 6;
            centaurScout.DamageBonus = 4;
            centaurScout.Defense = 3;
            centaurScout.Speed = 6;
            centaurScout.IsBackRow = false;
            centaurScout.MinGold = 100;
            centaurScout.MaxGold = 120;
            centaurScout.ExperienceReward = 18;
            centaurScout.Role = EnemyRole.Melee;
            centaurScout.EnergyRegenPerTurn = 2;
            centaurScout.AbilityNames.Add("Piercing Arrow");
            centaurScout.LootTable = new Dictionary<string, int> { {"potion", 35}, {"bronze gladius", 10} };
            npcs.Add(centaurScout.Name, centaurScout);

            NPC gorgon = new NPC();
            gorgon.Name = "Gorgon";
            gorgon.Description = "A serpentine woman with writhing snakes for hair and a petrifying gaze.";
            gorgon.ShortDescription = "A hostile gorgon";
            gorgon.IsHostile = true;
            gorgon.Health = 50;
            gorgon.MaxHealth = 50;
            gorgon.Energy = 25;
            gorgon.MaxEnergy = 25;
            gorgon.AttackDamage = 9;
            gorgon.DamageCount = 1;
            gorgon.DamageDie = 6;
            gorgon.DamageBonus = 5;
            gorgon.Defense = 4;
            gorgon.Speed = 4;
            gorgon.IsBackRow = true;
            gorgon.MinGold = 110;
            gorgon.MaxGold = 130;
            gorgon.ExperienceReward = 22;
            gorgon.Role = EnemyRole.Ranged;
            gorgon.EnergyRegenPerTurn = 2;
            gorgon.AbilityNames.Add("Entropy Bolt");
            gorgon.LootTable = new Dictionary<string, int> { {"potion", 40}, {"bronze bow", 10} };
            npcs.Add(gorgon.Name, gorgon);

            NPC bronzeAutomaton = new NPC();
            bronzeAutomaton.Name = "Bronze Automaton";
            bronzeAutomaton.Description = "A towering mechanical construct of bronze, eyes glowing with arcane energy.";
            bronzeAutomaton.ShortDescription = "A hostile bronze automaton";
            bronzeAutomaton.IsHostile = true;
            bronzeAutomaton.Health = 60;
            bronzeAutomaton.MaxHealth = 60;
            bronzeAutomaton.Energy = 10;
            bronzeAutomaton.MaxEnergy = 10;
            bronzeAutomaton.AttackDamage = 10;
            bronzeAutomaton.DamageCount = 1;
            bronzeAutomaton.DamageDie = 8;
            bronzeAutomaton.DamageBonus = 5;
            bronzeAutomaton.Defense = 7;
            bronzeAutomaton.Speed = 3;
            bronzeAutomaton.IsBackRow = false;
            bronzeAutomaton.MinGold = 120;
            bronzeAutomaton.MaxGold = 140;
            bronzeAutomaton.ExperienceReward = 25;
            bronzeAutomaton.Role = EnemyRole.Melee;
            bronzeAutomaton.EnergyRegenPerTurn = 1;
            bronzeAutomaton.LootTable = new Dictionary<string, int> { {"potion", 45}, {"bronze plate", 15} };
            npcs.Add(bronzeAutomaton.Name, bronzeAutomaton);

            NPC fury = new NPC();
            fury.Name = "Fury";
            fury.Description = "A winged demon wreathed in flames, shrieking curses from the depths of Tartarus.";
            fury.ShortDescription = "A hostile fury";
            fury.IsHostile = true;
            fury.Health = 48;
            fury.MaxHealth = 48;
            fury.Energy = 30;
            fury.MaxEnergy = 30;
            fury.AttackDamage = 10;
            fury.DamageCount = 1;
            fury.DamageDie = 6;
            fury.DamageBonus = 6;
            fury.Defense = 3;
            fury.Speed = 7;
            fury.IsBackRow = true;
            fury.MinGold = 105;
            fury.MaxGold = 125;
            fury.ExperienceReward = 20;
            fury.Role = EnemyRole.Ranged;
            fury.EnergyRegenPerTurn = 2;
            fury.AbilityNames.Add("Flame Bolt");
            fury.LootTable = new Dictionary<string, int> { {"potion", 40}, {"bronze staff", 10} };
            npcs.Add(fury.Name, fury);

            // Floor 3 Enemies (Level 11-15)
            NPC minotaur = new NPC();
            minotaur.Name = "Minotaur";
            minotaur.Description = "A massive bull-headed warrior wielding a great axe, snorting with rage.";
            minotaur.ShortDescription = "A hostile minotaur";
            minotaur.IsHostile = true;
            minotaur.Health = 75;
            minotaur.MaxHealth = 75;
            minotaur.Energy = 15;
            minotaur.MaxEnergy = 15;
            minotaur.AttackDamage = 13;
            minotaur.DamageCount = 1;
            minotaur.DamageDie = 10;
            minotaur.DamageBonus = 7;
            minotaur.Defense = 5;
            minotaur.Speed = 5;
            minotaur.IsBackRow = false;
            minotaur.MinGold = 160;
            minotaur.MaxGold = 180;
            minotaur.ExperienceReward = 40;
            minotaur.Role = EnemyRole.Melee;
            minotaur.EnergyRegenPerTurn = 2;
            minotaur.AbilityNames.Add("Crushing Blow");
            minotaur.LootTable = new Dictionary<string, int> { {"potion", 50}, {"enchanted greataxe", 12} };
            npcs.Add(minotaur.Name, minotaur);

            NPC medusa = new NPC();
            medusa.Name = "Medusa";
            medusa.Description = "The legendary gorgon queen with serpents writhing atop her head, her gaze deadly.";
            medusa.ShortDescription = "A hostile medusa";
            medusa.IsHostile = true;
            medusa.Health = 65;
            medusa.MaxHealth = 65;
            medusa.Energy = 40;
            medusa.MaxEnergy = 40;
            medusa.AttackDamage = 11;
            medusa.DamageCount = 1;
            medusa.DamageDie = 8;
            medusa.DamageBonus = 6;
            medusa.Defense = 4;
            medusa.Speed = 6;
            medusa.IsBackRow = true;
            medusa.MinGold = 155;
            medusa.MaxGold = 175;
            medusa.ExperienceReward = 42;
            medusa.Role = EnemyRole.Ranged;
            medusa.EnergyRegenPerTurn = 3;
            medusa.AbilityNames.Add("Entropy Bolt");
            medusa.AbilityNames.Add("Void Touch");
            medusa.LootTable = new Dictionary<string, int> { {"potion", 55}, {"enchanted bow", 12} };
            npcs.Add(medusa.Name, medusa);

            NPC cyclops = new NPC();
            cyclops.Name = "Cyclops";
            cyclops.Description = "A towering one-eyed giant wielding a massive club carved from a tree trunk.";
            cyclops.ShortDescription = "A hostile cyclops";
            cyclops.IsHostile = true;
            cyclops.Health = 85;
            cyclops.MaxHealth = 85;
            cyclops.Energy = 10;
            cyclops.MaxEnergy = 10;
            cyclops.AttackDamage = 15;
            cyclops.DamageCount = 1;
            cyclops.DamageDie = 12;
            cyclops.DamageBonus = 8;
            cyclops.Defense = 6;
            cyclops.Speed = 4;
            cyclops.IsBackRow = false;
            cyclops.MinGold = 170;
            cyclops.MaxGold = 190;
            cyclops.ExperienceReward = 45;
            cyclops.Role = EnemyRole.Melee;
            cyclops.EnergyRegenPerTurn = 2;
            cyclops.AbilityNames.Add("Power Attack");
            cyclops.LootTable = new Dictionary<string, int> { {"potion", 60}, {"enchanted plate", 15} };
            npcs.Add(cyclops.Name, cyclops);

            NPC lamia = new NPC();
            lamia.Name = "Lamia";
            lamia.Description = "A serpentine sorceress with a woman's torso and a snake's lower body, eyes glowing with dark magic.";
            lamia.ShortDescription = "A hostile lamia";
            lamia.IsHostile = true;
            lamia.Health = 60;
            lamia.MaxHealth = 60;
            lamia.Energy = 45;
            lamia.MaxEnergy = 45;
            lamia.AttackDamage = 9;
            lamia.DamageCount = 1;
            lamia.DamageDie = 6;
            lamia.DamageBonus = 5;
            lamia.Defense = 3;
            lamia.Speed = 8;
            lamia.IsBackRow = true;
            lamia.MinGold = 150;
            lamia.MaxGold = 170;
            lamia.ExperienceReward = 38;
            lamia.Role = EnemyRole.Ranged;
            lamia.EnergyRegenPerTurn = 3;
            lamia.AbilityNames.Add("Void Touch");
            lamia.AbilityNames.Add("Flame Bolt");
            lamia.LootTable = new Dictionary<string, int> { {"potion", 55}, {"enchanted staff", 12} };
            npcs.Add(lamia.Name, lamia);

            // Floor 4 Enemies (Level 16-20)
            NPC hydra = new NPC();
            hydra.Name = "Hydra";
            hydra.Description = "A multi-headed serpent with five snapping heads, each dripping with venom.";
            hydra.ShortDescription = "A hostile hydra";
            hydra.IsHostile = true;
            hydra.Health = 95;
            hydra.MaxHealth = 95;
            hydra.Energy = 30;
            hydra.MaxEnergy = 30;
            hydra.AttackDamage = 14;
            hydra.DamageCount = 2;
            hydra.DamageDie = 8;
            hydra.DamageBonus = 6;
            hydra.Defense = 7;
            hydra.Speed = 6;
            hydra.IsBackRow = false;
            hydra.MinGold = 210;
            hydra.MaxGold = 230;
            hydra.ExperienceReward = 70;
            hydra.Role = EnemyRole.Melee;
            hydra.EnergyRegenPerTurn = 2;
            hydra.AbilityNames.Add("Crushing Blow");
            hydra.AbilityNames.Add("Power Attack");
            hydra.LootTable = new Dictionary<string, int> { {"potion", 65}, {"legendary sword", 15} };
            npcs.Add(hydra.Name, hydra);

            NPC chimera = new NPC();
            chimera.Name = "Chimera";
            chimera.Description = "A monstrous hybrid with a lion's body, goat's head, and serpent's tail, breathing fire.";
            chimera.ShortDescription = "A hostile chimera";
            chimera.IsHostile = true;
            chimera.Health = 90;
            chimera.MaxHealth = 90;
            chimera.Energy = 40;
            chimera.MaxEnergy = 40;
            chimera.AttackDamage = 13;
            chimera.DamageCount = 2;
            chimera.DamageDie = 8;
            chimera.DamageBonus = 5;
            chimera.Defense = 6;
            chimera.Speed = 8;
            chimera.IsBackRow = false;
            chimera.MinGold = 205;
            chimera.MaxGold = 225;
            chimera.ExperienceReward = 68;
            chimera.Role = EnemyRole.Melee;
            chimera.EnergyRegenPerTurn = 3;
            chimera.AbilityNames.Add("Flame Bolt");
            chimera.AbilityNames.Add("Power Attack");
            chimera.LootTable = new Dictionary<string, int> { {"potion", 65}, {"legendary bow", 15} };
            npcs.Add(chimera.Name, chimera);

            NPC cerberus = new NPC();
            cerberus.Name = "Cerberus";
            cerberus.Description = "The legendary three-headed hound of Hades, its jaws dripping with ichor.";
            cerberus.ShortDescription = "A hostile cerberus";
            cerberus.IsHostile = true;
            cerberus.Health = 100;
            cerberus.MaxHealth = 100;
            cerberus.Energy = 25;
            cerberus.MaxEnergy = 25;
            cerberus.AttackDamage = 15;
            cerberus.DamageCount = 3;
            cerberus.DamageDie = 6;
            cerberus.DamageBonus = 5;
            cerberus.Defense = 8;
            cerberus.Speed = 9;
            cerberus.IsBackRow = false;
            cerberus.MinGold = 220;
            cerberus.MaxGold = 240;
            cerberus.ExperienceReward = 75;
            cerberus.Role = EnemyRole.Melee;
            cerberus.EnergyRegenPerTurn = 2;
            cerberus.AbilityNames.Add("Power Attack");
            cerberus.LootTable = new Dictionary<string, int> { {"potion", 70}, {"legendary plate", 18} };
            npcs.Add(cerberus.Name, cerberus);

            NPC titan = new NPC();
            titan.Name = "Titan";
            titan.Description = "A colossal primordial being, towering above all others with godlike strength.";
            titan.ShortDescription = "A hostile titan";
            titan.IsHostile = true;
            titan.Health = 110;
            titan.MaxHealth = 110;
            titan.Energy = 50;
            titan.MaxEnergy = 50;
            titan.AttackDamage = 16;
            titan.DamageCount = 2;
            titan.DamageDie = 10;
            titan.DamageBonus = 8;
            titan.Defense = 9;
            titan.Speed = 7;
            titan.IsBackRow = false;
            titan.MinGold = 240;
            titan.MaxGold = 260;
            titan.ExperienceReward = 100;
            titan.Role = EnemyRole.Melee;
            titan.EnergyRegenPerTurn = 3;
            titan.AbilityNames.Add("Power Attack");
            titan.AbilityNames.Add("Crushing Blow");
            titan.LootTable = new Dictionary<string, int> { {"potion", 75}, {"divine warhammer", 20} };
            npcs.Add(titan.Name, titan);

            return npcs;
        }
    }
}