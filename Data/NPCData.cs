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
                Text = "He sizes you up. \"You?\" He pauses, then continues, \"...Perhaps. The leader calls himself the Bandit Warlord. Pompous bastard. If you can kill him and bring me proof - his head will do - I'll convince the council to open the gate. The caves are south of the lower mountain slopes, room 12 if you're keeping track.\"",
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
                Text = "Marcus' eyes widen as you unwrap the grisly trophy. \"By the gods... you actually did it! The Warlord's head!\" He examines it grimly. \"This will send a message to any other bandits thinking of setting up shop here. Well done, adventurer. I'll inform the council immediately - the southern gate is now open to you.\"",
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
            priestess.Description = "A slender woman with fair skin and golden hair greets you with a smile.  While her face shows no signs of old age, her silver eyes seem to contain endless wisdom.  She is dressed simply in white robes, with a single silver armlet around her left bicep.";
            priestess.ShortDescription = "A priestess";

            priestess.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "The priestess turns towards you with a smile. \"Welcome. I am Caelia, Priestess of Keius. What brings you to his temple?\"",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "\"Just looking around, thank you.\"", nextNodeID = "end" }
                }
            });

            priestess.Dialogue.Add("end", new DialogueNode ()
            {
                Text = "Caelia gives you a quizzical look, then smiles softly. \"Very well. When you're ready, come see me again.\"",
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
            livia.AcceptDialogue = "Thank the gods. I won't let you down. These bandits will pay for what they've done.";

            // Add bandit cave NPCs (sentry/Marcus already added earlier)
            npcs.Add(banditScout.Name, banditScout);
            npcs.Add(banditCutthroat.Name, banditCutthroat);
            npcs.Add(banditEnforcer.Name, banditEnforcer);
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
            testRecruit4.Description = "A proud warrior crosses his arms and eyes you challengingly.";
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
            testRecruit4.YieldDialogue = "You fight well. I'll join your guild.";
            testRecruit4.AcceptDialogue = "I'll head to the guild hall. You've earned my respect.";
            testRecruit4.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "Prove your strength in combat, and I'll consider your guild!",
                Action = new DialogueAction { Type = "trigger_combat" },
                Choices = { }
            });

            // Test Recruit 5 - Venator, Female, Combat Recruit
            NPC testRecruit5 = new NPC();
            testRecruit5.Name = "Aria Swift";
            testRecruit5.Description = "A nimble archer twirls her bow, eager for a challenge.";
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
            testRecruit5.YieldDialogue = "Impressive! You're quick. I'll join.";
            testRecruit5.AcceptDialogue = "On my way to the guild hall!";
            testRecruit5.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "Let's see what you've got! En garde!",
                Action = new DialogueAction { Type = "trigger_combat" },
                Choices = { }
            });

            // Test Recruit 6 - Oracle, Male, Combat Recruit
            NPC testRecruit6 = new NPC();
            testRecruit6.Name = "Aldric the Wise";
            testRecruit6.Description = "An elderly mage taps his staff, arcane energies crackling around him.";
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
            testRecruit6.YieldDialogue = "Your magical prowess is... acceptable. I'll join.";
            testRecruit6.AcceptDialogue = "I shall lend my wisdom to your guild.";
            testRecruit6.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "A duel of magic, then! Prepare yourself!",
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

            return npcs;
        }
    }
}