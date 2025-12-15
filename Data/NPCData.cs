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
                Text = "Move along, citizen. Keep your nose clean and we won't have any problems.",
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
                Text = "Good day to you, traveler. Welcome to Belum!",
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
                Text = "Looking to buy? Looking to sell? Either way, you've come to the right place!",
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
                Text = "Welcome to the Golden Grape! What'll it be? Ale? Wine? Or perhaps you're looking for information?",
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
                Text = "Need something forged? Repaired? I'm the best smith in Belum - ask anyone.",
                Choices = { }
            });
            npcs.Add(blacksmith.Name, blacksmith);
            
            
            NPC farmer = new NPC();
            farmer.Name = "Gaius";
            farmer.Description = "A burly man of over two meters leans against against one of the four posts of his small stall. As he notices you, he regards you with a mixture of kindness and mild surprise.";
            farmer.ShortDescription = "A farmer";

            farmer.Dialogue.Add("main_hub", new DialogueNode()
            {
                Text = "Is there anything I can help you with? ",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "What can you tell me about the area?", nextNodeID = "ask_about_area" },
                    new DialogueNode.Choice { choiceText = "Actually, I'm taking over that old guildhouse.", nextNodeID = "explain_guild" },
                    new DialogueNode.Choice { choiceText = "Tell me more about this dangerous forest.", nextNodeID = "ask_about_forest" },
                    new DialogueNode.Choice { choiceText = "Thanks for the warning. I should get going.", nextNodeID = "goodbye" }
                }
            });

            farmer.Dialogue.Add("ask_about_area", new DialogueNode()
            {
                Text = "I thought you looked new.  I assume you've been to Belum, the town to the north, but if you head west you'll pass my farm on the way to the mountains.  East of here is a nasty forest, I'd stay out of there if I were you - unless you're looking for trouble, I suppose.  What were you doing down by the old guildhouse?  Get lost?",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Actually, I'm taking over that old guildhouse.", nextNodeID = "explain_guild" },
                    new DialogueNode.Choice { choiceText = "Tell me more about this dangerous forest.", nextNodeID = "ask_about_forest" },
                    new DialogueNode.Choice { choiceText = "Thanks for the warning. I should get going.", nextNodeID = "goodbye" },
                    new DialogueNode.Choice { choiceText = "Talk about something else.", nextNodeID = "main_hub"}
                }
            });

            farmer.Dialogue.Add("explain_guild", new DialogueNode()
            {
                Text = "A guild? Here? Ha! That old place has been empty for years. But... I suppose someone with ambition could make something of it. You seem determined enough.",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Talk about something else.", nextNodeID = "main_hub"}
                }
            });

            farmer.Dialogue.Add("recruit", new DialogueNode()
            {
                Text = "Recuits?  For that old guildhall?  Hah-" +
                "\n\n  [the farmer coughs mid-laughter, and enters a brief fit of wheezing] " +
                "\nSorry about that..  you might find someone in there - I saw a ranger go that way earlier.",

                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Talk about something else.", nextNodeID = "main_hub"}
                }
            });

            farmer.Dialogue.Add("ask_about_forest", new DialogueNode()
            {
                Text = "Dark woods, full of wolves and worse things. Lost a few chickens to whatever lurks in there. Some say there's treasure hidden deep inside, but I say it's not worth the risk.",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Maybe that's where I'll find my first recruits.", nextNodeID = "recruit" },
                    new DialogueNode.Choice { choiceText = "I'll stay clear of it then.", nextNodeID = "goodbye" },
                    new DialogueNode.Choice { choiceText = "Talk about something else.", nextNodeID = "main_hub"}
                }
            });

            farmer.Dialogue.Add("goodbye", new DialogueNode()
            {
                Text = "Safe travels, friend. Stop by my farm sometime if you're heading west.",
                Choices = { } // empty = conversation ends
            });


            NPC ranger = new NPC();
            ranger.Name = "Silvacis";
            ranger.Description = "A tall, slender man with light hair is here, digging through the mud rather frantically.  Suddenly, his head raises and turns towards you with a scowl.";
            ranger.ShortDescription = "A ranger";

            // Silvacis dialogue restructure
            ranger.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "Hey! You there! Wait, you're not from around here... What brings someone new to these dangerous woods?",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "I'm rebuilding the Adventurer's Guild nearby.", nextNodeID = "mention_guild_first" },
                    new DialogueNode.Choice { choiceText = "Just exploring the area.", nextNodeID = "exploring" },
                    new DialogueNode.Choice { choiceText = "That's my business.", nextNodeID = "rude_response" }
                }
            });

            ranger.Dialogue.Add("mention_guild_first", new DialogueNode()
            {
                Text = "The old guild hall? Interesting... I could use the help of a guild actually. I've lost something important - a weather-worn silver amulet. I dropped it somewhere in this cursed forest and I can't find it anywhere!",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "Is this it? (Give amulet)",
                        nextNodeID = "give_amulet_guild_known",
                        IsAvailable = (inventory) => inventory.Contains("amulet"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "amulet"} } }
                    },
                    new DialogueNode.Choice { choiceText = "What's so special about this amulet?", nextNodeID = "amulet_importance" },
                    new DialogueNode.Choice { choiceText = "I'll keep an eye out for it.", nextNodeID = "will_search" }
                }
            });

            ranger.Dialogue.Add("exploring", new DialogueNode()
            {
                Text = "Be careful then. These woods are dangerous... Actually, since you're here, maybe you can help me. I've lost something important - a weather-worn silver amulet. Have you seen it?",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "Is this it? (Give amulet)",
                        nextNodeID = "give_amulet_no_guild",
                        IsAvailable = (inventory) => inventory.Contains("amulet"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "amulet"} } }
                    },
                    new DialogueNode.Choice { choiceText = "What kind of amulet?", nextNodeID = "describe_amulet_no_guild" },
                    new DialogueNode.Choice { choiceText = "Sorry, haven't seen it.", nextNodeID = "goodbye" }
                }
            });

            ranger.Dialogue.Add("rude_response", new DialogueNode()
            {
                Text = "Fair enough... Look, I don't mean to bother you, but I've lost something important. A silver amulet. If you find it, please let me know.",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Fine, I'll keep an eye out.", nextNodeID = "goodbye" }
                }
            });

            ranger.Dialogue.Add("give_amulet_guild_known", new DialogueNode()
            {
                Text = "That's it! That's my amulet! Thank you so much! You know what? A guild sounds perfect - I've been alone in these woods too long. An Adventurer's Guild is exactly what I need!",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "Welcome aboard! Head to the guild hall south of the crossroads.",
                        nextNodeID = "recruit_offer"
                    }
                }
            });

            ranger.Dialogue.Add("give_amulet_no_guild", new DialogueNode()
            {
                Text = "That's it! That's my amulet! Thank you so much! I owe you a great debt... Say, you seem capable. I don't suppose you're looking for companions? I'm a skilled ranger, and I've been thinking it's time to leave these woods.",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Actually, I'm rebuilding the Adventurer's Guild. Interested?", nextNodeID = "reveal_guild" },
                    new DialogueNode.Choice { choiceText = "I work alone.", nextNodeID = "decline_company" }
                }
            });

            ranger.Dialogue.Add("reveal_guild", new DialogueNode()
            {
                Text = "An Adventurer's Guild? That's perfect! I'd be honored to join. It's been too long since I had a real purpose.",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "Welcome aboard! Head to the guild hall south of the crossroads.",
                        nextNodeID = "recruit_offer"
                    }
                }
            });

            ranger.Dialogue.Add("amulet_importance", new DialogueNode()
            {
                Text = "It belonged to someone important to me. It's all I have left of them... Please, if you find it, bring it to me.",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "Is this it? (Give amulet)",
                        nextNodeID = "give_amulet_guild_known",
                        IsAvailable = (inventory) => inventory.Contains("amulet"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "amulet"} } }
                    },
                    new DialogueNode.Choice { choiceText = "I'll look for it.", nextNodeID = "will_search" }
                }
            });

            ranger.Dialogue.Add("will_search", new DialogueNode()
            {
                Text = "Thank you. I'll keep searching here. If you find it, please bring it to me.",
                Choices = { }
            });

            // Main hub for return visits
            ranger.Dialogue.Add("main_hub", new DialogueNode()
            {
                Text = "Have you found my amulet yet? It's a weather-worn silver piece, very important to me.",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "Give the amulet",
                        nextNodeID = "give_amulet_return",
                        IsAvailable = (inventory) => inventory.Contains("amulet"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "amulet"} } }
                    },
                    new DialogueNode.Choice { choiceText = "Still looking for it.", nextNodeID = "goodbye" }
                }
            });

            ranger.Dialogue.Add("give_amulet_return", new DialogueNode()
            {
                Text = "My amulet! Thank you! You know, I've been thinking about what you said about that guild. I'd like to join, if you'll have me.",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "Welcome to the guild!",
                        nextNodeID = "recruit_offer"
                    }
                }
            });

            // Keep the existing recruit_offer
            ranger.Dialogue.Add("recruit_offer", new DialogueNode()
            {
                Text = "I'll head to the guild hall right away. Oh, and if you're looking for more recruits, I heard there's a fighter who camps deeper in these woods - bit of a hermit, but skilled with a blade.",
                Action = new DialogueAction { Type = "add_recruit", Parameters = { { "class", "Venator" } } },
                Choices = { }
            });

            ranger.Dialogue.Add("goodbye", new DialogueNode()
            {
                Text = "Please, if you find it, let me know. I'll be searching around here.",
                Choices = { } // conversation ends
            });

            // Also add the "decline_company" node that was referenced but missing:
            ranger.Dialogue.Add("decline_company", new DialogueNode()
            {
                Text = "I understand. But please, keep the amulet as thanks. And if you ever change your mind about needing help, I'll be around.",
                Choices = { } // conversation ends  
            });

            // And "describe_amulet_no_guild" that was also referenced:
            ranger.Dialogue.Add("describe_amulet_no_guild", new DialogueNode()
            {
                Text = "It's a weather-worn silver amulet, quite old. It belonged to someone important to me. It's all I have left of them.",
                Choices =
                {
                    new DialogueNode.Choice {
                        choiceText = "Is this it? (Give amulet)",
                        nextNodeID = "give_amulet_no_guild",
                        IsAvailable = (inventory) => inventory.Contains("amulet"),
                        Action = new DialogueAction { Type = "give_item", Parameters = { {"item", "amulet"} } }
                    },
                    new DialogueNode.Choice { choiceText = "I'll keep an eye out for it.", nextNodeID = "goodbye" }
                }
            });


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
                Text = "Leave me be, stranger. I came to these woods for solitude, not conversation.",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "I'm rebuilding the Adventurer's Guild. Interested?", nextNodeID = "reject_guild" },
                    new DialogueNode.Choice { choiceText = "Fair enough. I'll leave you alone.", nextNodeID = "goodbye" }
                }
            });

            fighter.Dialogue.Add("reject_guild", new DialogueNode()
            {
                Text = "A guild? Ha! I'm done taking orders. If you want me to join anything, you'll have to prove you're worth following. Draw your weapon!",
                Action = new DialogueAction { Type = "trigger_combat" },
                Choices = { }
            });

            fighter.Dialogue.Add("goodbye", new DialogueNode()
            {
                Text = "Smart choice. Now leave me be.",
                Choices = { }
            });

            farmer.Dialogue.Add("greeting", new DialogueNode()
            {
                Text = "Greetings, friend. I don't recall seeing you come through here before.  ",
                Choices =
                {
                    new DialogueNode.Choice { choiceText = "Introduce yourself", nextNodeID = "main_hub"}
                }
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

            return npcs;
        }
    }
}