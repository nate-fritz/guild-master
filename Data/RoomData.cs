using GuildMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildMaster.Data
{
    public static class RoomData
    {
        public static Dictionary<int, Room> InitializeRooms(Dictionary<string, NPC> npcs)
        {
            var rooms = new Dictionary<int, Room>();

            // Updated
            Room bedroom = CreateRoom(1, "guildHallBedroom", "The Guild Hall - Bedroom", "You're in a small bedroom with a bed and a nightstand.  There's a small locked chest at the foot of the bed.  There's a desk with a small copper mug sitting on it. To the east is a door that leads to a hallway.");
            bedroom.Exits.Add("east", 2);
            bedroom.Items.Add("mug");
            bedroom.Items.Add("note");
            bedroom.Items.Add("chest");

            // Updated
            Room hallway = CreateRoom(2, "guildHallHallway", "The Guild Hall - Hallway", "You're in a narrow hallway that runs north to south.  To the west is your bedroom.  A study is to the south, and a large common area is open to the north.");
            hallway.Exits.Add("west", 1);
            hallway.Exits.Add("north", 4);
            hallway.Exits.Add("south", 3);

            // Updated
            Room study = CreateRoom(3, "guildHallStudy", "The Guild Hall - Study", "You're in a dusty study filled with old books, scrolls, and other trinkets.  The hallway that you entered from is to the north.");
            study.Exits.Add("north", 2);
            study.Items.Add("desk");
            study.Items.Add("restoration scroll");
            study.Items.Add("dusty tome");


            // Updated
            Room commonArea = CreateRoom(4, "guildHallCommonArea", "The Guild Hall - Common Area", "A large common area with tables, chairs, and couches in front of a large fireplace.  You imagine this room was once quite lively, but today it sits unused.  The main door to the guildhall is to the north, and a hallway is to the south.");
            commonArea.Exits.Add("south", 2);
            commonArea.Exits.Add("north", 5);
            // Dynamic descriptions based on guild size
            commonArea.DescriptionVariants.Add("recruits_4", "A large common area with tables, chairs, and couches arranged before a roaring fireplace. The space is beginning to show signs of life - a few members can be seen relaxing between missions. The main door to the guildhall is to the north, and a hallway is to the south. A passage to the west leads to the newly opened training yard.");
            commonArea.DescriptionVariants.Add("recruits_6", "A lively common area filled with the sounds of conversation and camaraderie. Tables and chairs surround a roaring fireplace where guild members gather to share tales of their adventures. The room feels alive with purpose. The main door is to the north, the hallway to the south. Passages lead west to the training yard and east to the armory.");
            commonArea.DescriptionVariants.Add("recruits_8", "A bustling common area that serves as the heart of your thriving guild. Members fill the space, some relaxing by the fireplace, others planning their next quest. The energy is palpable. The main door is to the north, the hallway to the south, with passages leading west to the training yard and east to the armory.");
            commonArea.DescriptionVariants.Add("recruits_10", "The vibrant heart of a legendary guild hall. This grand common area is filled with elite adventurers, their equipment gleaming in the firelight. Trophies from epic quests line the walls. The main door is to the north, the hallway to the south, with passages leading west to the training yard and east to the armory. Your guild has achieved greatness.");

            // Updated
            Room frontDoor = CreateRoom(5, "guildHallFrontDoor", "The Guild Hall - Front Door", "You stand before a modest two-story building made of grey stone with a yellow thatched roof. Ivy climbs the walls, and the roof looks a bit worse for wear, but considering it's been largely abandoned in recent years, it's relatively well maintained. Two large wooden doors stand at the back of the portico at the front of the guildhall. A hanging sign sways lazily in the wind, its painted lettering worn away to the point of being indecipherable.");
            frontDoor.Exits.Add("south", 4);
            frontDoor.Exits.Add("north", 6);

            // Progressive Guild Rooms (unlock based on recruit count)
            // Room 64: Training Yard (unlocks with 4 recruits)
            Room trainingYard = CreateRoom(64, "guildHallTrainingYard", "The Guild Hall - Training Yard", "A spacious outdoor training yard behind the guild hall. Wooden practice dummies line one wall, their surfaces scarred from countless strikes. A weapons rack holds an assortment of training weapons, and the packed earth shows signs of recent use. Your guild is growing, and this space gives your warriors room to hone their skills.");
            trainingYard.Exits.Add("east", 4);  // Back to Common Area

            // Room 65: Armory (unlocks with 6 recruits)
            Room armory = CreateRoom(65, "guildHallArmory", "The Guild Hall - Armory", "A well-stocked armory filled with weapon racks and armor stands. The smell of oil and leather fills the air. Light streams through narrow windows, glinting off polished steel. With your guild's reputation growing, a mysterious armorer has taken up residence here, offering quality equipment to those with coin to spare.");
            armory.Exits.Add("west", 4);  // Back to Common Area
            armory.NPCs.Add(npcs["Guild Armorer"].Clone());  // Add the guild armorer
            // East exit to Treasury (66) added conditionally when 8 recruits reached

            // Room 66: Treasury (unlocks with 8 recruits)
            Room treasury = CreateRoom(66, "guildHallTreasury", "The Guild Hall - Treasury", "A secure treasury room lined with reinforced chests and lockboxes. The walls are thick stone, and a heavy iron door guards the entrance. Your guild's wealth and most valuable treasures are stored here. In the center of the room sits an ornate chest containing rings of power, one for each class of adventurer.");
            treasury.Exits.Add("west", 65);  // Back to Armory
            treasury.Items.Add("legionnaire's ring");
            treasury.Items.Add("venator's ring");
            treasury.Items.Add("oracle's ring");
            // South exit to Portal Room (67) added conditionally when 10 recruits reached

            // Room 67: Portal Room (unlocks with 10 recruits)
            Room portalRoom = CreateRoom(67, "guildHallPortalRoom", "The Guild Hall - Portal Chamber", "A mystical chamber thrumming with arcane energy. Three shimmering portals stand in alcoves along the walls, their surfaces rippling like water. Ancient runes are carved into the floor in concentric circles, glowing with a faint blue light. Your guild has achieved greatness, and these portals now offer passage to distant lands that would otherwise take weeks to reach.");
            portalRoom.Exits.Add("north", 66);  // Back to Treasury
            // Portal exits to far-off regions
            portalRoom.Exits.Add("east", 80);   // Portal to Belum Town Square
            portalRoom.Exits.Add("west", 25);   // Portal to Mountain Peak (Mount Gelus)
            portalRoom.Exits.Add("down", 61);   // Portal to Eastern Forest Exit (Hircinian Forest)

            // Updated
            Room guildPath = CreateRoom(6, "guildPath", "A Dirt Path", "You're on a wide dirt path that runs roughly north to south. Rolling grass covered hills flank either side of the path, with the occasional tree breaking up the sea of green.  To the south you can still see the guild hall.  Further north, it looks like another road intersects with the one that you're on.");
            guildPath.Exits.Add("south", 5);
            guildPath.Exits.Add("north", 7);

            // Updated
            Room theCrossRoads = CreateRoom(7, "theCrossRoads", "A crossroads", "You enter a crossroads that extends in the four cardinal directions.  A wooden sign post has four signs pointing towards each. <br><br>North:   The Town of Belum<br>East:    Hircinian Forest<br>South:   [this sign looks long neglected, and can't be read]<br>West:    Avinten Foothills");
            theCrossRoads.Exits.Add("south", 6);
            theCrossRoads.Exits.Add("west", 8);
            theCrossRoads.Exits.Add("east", 40);
            theCrossRoads.Exits.Add("north", 68);
            theCrossRoads.Items.Add("neglected sign");
            theCrossRoads.NPCs.Add(npcs["Gaius"]);  // Gaius is unique, no clone needed

            // Updated
            Room westPath = CreateRoom(8, "westPath", "A path west of the crossroads", "You are west of The Crossroads on a wide dirt path.  In some areas, where the road is deeply pitted from use and weather, one might notice ancient cobblestone.  Further west the skyline is dominated by distant snow tipped mountains.");
            westPath.Exits.Add("east", 7);
            westPath.Exits.Add("west", 9);

            // Updated
            Room westernBend = CreateRoom(9, "westernBend", "A bend in the road", "The road bends sharply before resuming a west-to-east orientation.  To the west you see a nearby mountain range.  To the far east you can barely make out a crossroads.");
            westernBend.Exits.Add("east", 8);
            westernBend.Exits.Add("west", 10);
            westernBend.NPCs.Add(npcs["Bandit"].Clone());
            westernBend.CanRespawn = true;
            westernBend.RespawnTimeHours = 16f;
           
            westernBend.OriginalNPCs.Add(npcs["Bandit"].Clone());

            // Updated
            Room gaiusFarmFields = CreateRoom(10, "gaiusFarmFields", "A farm in the Avinten foothills", "You see fields of grain extending to the north and vineyards to the south as far as your eye can see.  Further west you see graneries and barns, stables and sheds for livestock.");
            gaiusFarmFields.Exits.Add("east", 9);
            gaiusFarmFields.Exits.Add("west", 11);
            gaiusFarmFields.CanRespawn = true;
            gaiusFarmFields.RespawnTimeHours = 16f;
            gaiusFarmFields.OriginalNPCs.Add(npcs["Bandit"].Clone());
            gaiusFarmFields.OriginalNPCs.Add(npcs["Bandit Thug"].Clone());
            gaiusFarmFields.NPCs.Add(new NPC()
            {
                Name = "Bandit",
                Description = "A rough-looking bandit blocks your path, weapon drawn.",
                ShortDescription = "A hostile bandit",
                IsHostile = true,
                Health = 8,
                MaxHealth = 8,
                AttackDamage = 2,
                Defense = 0,
                Speed = 2,
                MinGold = 1,
                MaxGold = 3,
                LootTable = new Dictionary<string, int>
                {
                    {"potion", 30},
                    {"iron gladius", 5}
                },
                Dialogue = new Dictionary<string, DialogueNode>()
            });
            gaiusFarmFields.NPCs.Add(new NPC()
            {
                Name = "Bandit Thug",
                Description = "A rougher-looking bandit thug, weapon drawn.",
                ShortDescription = "A hostile bandit thug",
                IsHostile = true,
                Health = 12,
                MaxHealth = 12,
                AttackDamage = 3,
                Defense = 0,
                Speed = 1,
                MinGold = 2,
                MaxGold = 4,
                LootTable = new Dictionary<string, int>
                {
                    {"potion", 40},
                    {"iron gladius", 10}
                },
                Dialogue = new Dictionary<string, DialogueNode>()
            });

            // Updated
            Room gaiusFarmHouse = CreateRoom(11, "gaiusFarmHouse", "Gaius' Farmhouse", "A large stone house with baked clay roof tiles stands before you, surrounded by the fields and other buildings that make up Gaius' farm.  To the east are the fields, and to the west the land begins to climb upwards towards the peak of Mount Gelus.");
            gaiusFarmHouse.Exits.Add("east", 10);
            gaiusFarmHouse.Exits.Add("west", 12);
            gaiusFarmHouse.NPCs.Add(npcs["Bandit Leader"]);
            gaiusFarmHouse.CanRespawn = true;
            gaiusFarmHouse.RespawnTimeHours = 16f;
            gaiusFarmHouse.OriginalNPCs.Add(npcs["Bandit Leader"].Clone());

            // Updated
            Room lowerSlopes = CreateRoom(12, "lowerSlopes", "The Lower Slopes", "Here the rolling fields of the foothills meet the base of Mount Gelus, which begins to slope sharply upward to the west.  The mountain's snow-touched peaks are nearly hidden by wispy clouds above.  The steep incline and high winds make a trek up the mountain seem ill advised.  To the south you see a cave mouth, and to the east is Gaius' farm.");
            lowerSlopes.Exits.Add("east", 11);
            lowerSlopes.Exits.Add("west", 22);
            lowerSlopes.Exits.Add("south", 13); 

            // Updated
            Room caveEntrance = CreateRoom(13, "caveEntrance", "Bandit Cave - Entrance", "You approach the mouth of a cave that extends deep into the roots of Mount Gelus.  Flickering torches line the walls, and the smell of smoke and warm bodies hangs in the air.  Back to the north are the lower slopes of Mount Gelus, while the cave slopes downward to the south.");
            caveEntrance.Exits.Add("north", 12);  // Back to lower slopes
            caveEntrance.Exits.Add("south", 14);
            caveEntrance.NPCs.Add(npcs["Bandit Scout"].Clone());
            caveEntrance.NPCs.Add(npcs["Bandit Scout"].Clone());

            // Updated
            Room caveTunnel = CreateRoom(14, "caveTunnel", "Bandit Cave - Tunnel", "The tunnel slopes downward, the ceiling dripping with moisture. Crude wooden supports brace the walls. You hear voices echoing from deeper in the cave. The passage splits here - continuing south or branching west.");
            caveTunnel.Exits.Add("north", 13);
            caveTunnel.Exits.Add("south", 19);
            caveTunnel.Exits.Add("west", 15);
            caveTunnel.NPCs.Add(npcs["Bandit Scout"].Clone());
            caveTunnel.NPCs.Add(npcs["Bandit Cutthroat"].Clone());

            // Updated
            Room westernBranch = CreateRoom(15, "westernBranch", "Bandit Cave - Western Branch", "This narrower passage winds westward. Makeshift sleeping areas line the walls - dirty bedrolls and stolen goods scattered about. Someone has been living here. The passage continues west and opens into a larger chamber.");
            westernBranch.Exits.Add("east", 14);
            westernBranch.Exits.Add("west", 16);
            westernBranch.NPCs.Add(npcs["Bandit Cutthroat"].Clone());
            westernBranch.NPCs.Add(npcs["Bandit Cutthroat"].Clone());

            // Updated
            Room westernChamber = CreateRoom(16, "westernChamber", "Bandit Cave - Western Chamber", "A natural chamber opens up here, its high ceiling lost in darkness. Crates of stolen supplies are stacked against the walls - food, weapons, and trade goods taken from travelers. The bandits have made this their storage room. The tunnel continues west into deeper darkness.");
            westernChamber.Exits.Add("east", 15);
            westernChamber.Exits.Add("south", 17);
            westernChamber.NPCs.Add(npcs["Bandit Enforcer"].Clone());
            westernChamber.NPCs.Add(npcs["Bandit Cutthroat"].Clone());

            // Updated
            Room darkPassage = CreateRoom(17, "darkPassage", "Bandit Cave - Dark Passage", "The torches are fewer here, leaving long stretches of shadow. Your footsteps echo ominously. Blood stains mark the floor, evidence of past violence. The passage slopes further downward to the south, while a faint light glows from the north.");
            darkPassage.Exits.Add("north", 16);
            darkPassage.Exits.Add("south", 18);
            darkPassage.NPCs.Add(npcs["Bandit Enforcer"].Clone());

            // Updated
            Room deepCave = CreateRoom(18, "deepCave", "Bandit Cave - Deep Cavern", "You've descended deep into the mountain. The air here is cold and still. Strange rock formations create eerie shapes in the torchlight. Water drips steadily somewhere in the darkness. A massive iron gate blocks the passage east. The way north leads back toward the cave exit.");
            deepCave.Exits.Add("north", 17);
            // deepCave.Exits.Add("east", 21);  // Blocked by iron gate - unlocked with both keys
            deepCave.NPCs.Add(npcs["Bandit Guard"].Clone());  // Single guard with iron key
            deepCave.PuzzleId = "warlord_chamber_gates";

            deepCave.Objects.Add(new RoomObject
            {
                Id = "iron_gate",
                Name = "iron gate",
                Aliases = new[] { "gate", "iron door", "bars" },
                DefaultDescription = "A formidable iron gate bars the passage east. It has two keyholes - one for an iron key and one for a bronze key.",
                LookedAtDescription = "The gate is locked tight with a complex dual-lock mechanism. You'll need both an iron key and a bronze key to open it.",
                IsInteractable = true,
                InteractionType = "use",
                RequiredItem = null  // Handled by puzzle logic
            });

            // Updated
            Room floodedChamber = CreateRoom(19, "floodedChamber", "Bandit Cave - Flooded Chamber", "An underground stream flows through this chamber, the water black and swift. A narrow ledge runs along the eastern wall. Someone is standing guard here - a woman with a bow. She doesn't look like the other bandits. The only way forward is west, across a rickety wooden bridge.");
            floodedChamber.Exits.Add("north", 14);
            floodedChamber.Exits.Add("south", 20);
            floodedChamber.NPCs.Add(npcs["Livia"].Clone());  // Recruitable Venator

            // Updated
            Room undergroundRiver = CreateRoom(20, "undergroundRiver", "Bandit Cave - Underground River", "A narrow chasm opens up in the middle of this cavern, with a rickety looking wood and rope bridge spanning across it. It's too dark to see the bottom of the chasm, but you can faintly hear the flowing water of an underground river far below you. A bronze gate blocks the passage west. To the north is the way back toward the cave's entrance.");
            undergroundRiver.Exits.Add("north", 19);
            // undergroundRiver.Exits.Add("west", 21);  // Blocked by bronze gate - unlocked with both keys
            undergroundRiver.PuzzleId = "warlord_chamber_gates";

            undergroundRiver.Objects.Add(new RoomObject
            {
                Id = "bronze_gate",
                Name = "bronze gate",
                Aliases = new[] { "gate", "bronze door", "bars" },
                DefaultDescription = "An ornate bronze gate blocks the passage west. It has two keyholes - one for an iron key and one for a bronze key.",
                LookedAtDescription = "The gate is secured with a complex dual-lock mechanism. You'll need both an iron key and a bronze key to open it.",
                IsInteractable = true,
                InteractionType = "use",
                RequiredItem = null  // Handled by puzzle logic
            });

            Room warlordChamber = CreateRoom(21, "warlordChamber", "Bandit Cave - Warlord's Lair", "The passage opens into a large natural cavern that has been converted into a throne room of sorts. Stolen tapestries hang on the walls, and a crude throne of piled crates sits at the far end.");
            warlordChamber.Exits.Add("east", 20);
            warlordChamber.Exits.Add("west", 18);
            warlordChamber.NPCs.Add(npcs["Bandit Warlord"].Clone());  // Boss
            warlordChamber.NPCs.Add(npcs["Bandit Enforcer"].Clone());  // Guards
            warlordChamber.NPCs.Add(npcs["Bandit Enforcer"].Clone());

        
            // ALL OF THIS NEEDS UPDATED UNLESS MARKED OTHERWISE
            Room mountainPath = CreateRoom(22, "mountainPath", "Mountain Path", "A narrow path winds up the mountainside. The air grows thinner and colder as you climb. Loose rocks make footing treacherous. To the east you can see Gaius' farm far below. The path continues west, climbing higher.");
            mountainPath.Exits.Add("east", 12);
            mountainPath.Exits.Add("west", 23);
            mountainPath.NPCs.Add(npcs["Mountain Bandit"].Clone());
            mountainPath.NPCs.Add(npcs["Mountain Bandit"].Clone());
            mountainPath.NPCs.Add(npcs["Mountain Bandit"].Clone());
            mountainPath.CanRespawn = true;
            mountainPath.RespawnTimeHours = 12f;
            mountainPath.OriginalNPCs.Add(npcs["Mountain Bandit"].Clone());
            mountainPath.OriginalNPCs.Add(npcs["Mountain Bandit"].Clone());
            mountainPath.OriginalNPCs.Add(npcs["Mountain Bandit"].Clone());

            Room rockyOutcrop = CreateRoom(23, "rockyOutcrop", "Rocky Outcrop", "The path opens onto a wide rocky ledge. Ice and snow cover the ground here, and the temperature has dropped noticeably. The howling wind carries the sound of distant wolves. To the east the path descends, while to the west a dark cave entrance beckons.");
            rockyOutcrop.Exits.Add("east", 22);
            rockyOutcrop.Exits.Add("west", 24);
            rockyOutcrop.NPCs.Add(npcs["Frost Wolf"].Clone());
            rockyOutcrop.NPCs.Add(npcs["Frost Wolf"].Clone());
            rockyOutcrop.NPCs.Add(npcs["Frost Wolf"].Clone());
            rockyOutcrop.CanRespawn = true;
            rockyOutcrop.RespawnTimeHours = 12f;
            rockyOutcrop.OriginalNPCs.Add(npcs["Frost Wolf"].Clone());
            rockyOutcrop.OriginalNPCs.Add(npcs["Frost Wolf"].Clone());
            rockyOutcrop.OriginalNPCs.Add(npcs["Frost Wolf"].Clone());

            Room iceCavern = CreateRoom(24, "iceCavern", "Ice Cavern", "You enter a cavern entirely encased in ice. Frozen waterfalls create pillars of crystalline beauty. Strange shapes move within the ice walls - elementals of pure winter. The cave continues deeper to the west, while the entrance lies to the east.");
            iceCavern.Exits.Add("north", 25);
            iceCavern.Exits.Add("east", 23);
            iceCavern.NPCs.Add(npcs["Ice Elemental"].Clone());
            iceCavern.NPCs.Add(npcs["Ice Elemental"].Clone());
            iceCavern.NPCs.Add(npcs["Ice Elemental"].Clone());
            iceCavern.NPCs.Add(npcs["Ice Elemental"].Clone());
            iceCavern.CanRespawn = true;
            iceCavern.RespawnTimeHours = 12f;
            iceCavern.OriginalNPCs.Add(npcs["Ice Elemental"].Clone());
            iceCavern.OriginalNPCs.Add(npcs["Ice Elemental"].Clone());
            iceCavern.OriginalNPCs.Add(npcs["Ice Elemental"].Clone());
            iceCavern.OriginalNPCs.Add(npcs["Ice Elemental"].Clone());

            Room mountainPeak = CreateRoom(25, "mountainPeak", "Mountain Peak", "You emerge from the cave onto the mountain peak. Clouds swirl around you at this altitude, and lightning crackles in the distance. Massive eagles circle overhead, their cries echoing across the peaks. To the east is the ice cavern. To the west, ancient stone steps lead to what appears to be ruins.");
            mountainPeak.Exits.Add("north", 26);
            mountainPeak.Exits.Add("south", 24);
            mountainPeak.NPCs.Add(npcs["Thunder Eagle"].Clone());
            mountainPeak.NPCs.Add(npcs["Thunder Eagle"].Clone());
            mountainPeak.NPCs.Add(npcs["Thunder Eagle"].Clone());
            mountainPeak.CanRespawn = true;
            mountainPeak.RespawnTimeHours = 12f;
            mountainPeak.OriginalNPCs.Add(npcs["Thunder Eagle"].Clone());
            mountainPeak.OriginalNPCs.Add(npcs["Thunder Eagle"].Clone());
            mountainPeak.OriginalNPCs.Add(npcs["Thunder Eagle"].Clone());

            Room ancientAltar = CreateRoom(26, "ancientAltar", "Ancient Altar", "At the summit stands an ancient altar, carved from a single piece of black stone. Runes glow with an eerie light. A massive warrior in ancient armor stands before the altar, flanked by two elite guards. They turn to face you as you approach. The only exit is back to the east.");
            ancientAltar.Exits.Add("south", 25);
            ancientAltar.NPCs.Add(npcs["Mountain Warlord"].Clone());
            ancientAltar.NPCs.Add(npcs["Elite Guard"].Clone());
            ancientAltar.NPCs.Add(npcs["Elite Guard"].Clone());
            ancientAltar.CanRespawn = true;
            ancientAltar.RespawnTimeHours = 96f; // Boss takes longer to respawn
            ancientAltar.OriginalNPCs.Add(npcs["Mountain Warlord"].Clone());
            ancientAltar.OriginalNPCs.Add(npcs["Elite Guard"].Clone());
            ancientAltar.OriginalNPCs.Add(npcs["Elite Guard"].Clone());

            Room eastPath = CreateRoom(40, "eastPath", "A path east of the crossroads", "You are east of the crossroads on a wide dirt path. In some areas, where the road is deeply pitted from use and weather, one might notice ancient cobblestone.  Further east you see a dark and gnarled forest.");
            eastPath.Exits.Add("west", 7);
            eastPath.Exits.Add("east", 41);

            Room woodedPath = CreateRoom(41, "woodedPath", "A wooded path", "This section of the path is sparsely surrounded by trees and overgrowth.  To the distant west you see a crossroads and beyond that, a mountain range.  Further east the forest grows thicker.");
            woodedPath.Exits.Add("west", 40);
            woodedPath.Exits.Add("east", 42);
            woodedPath.NPCs.Add(npcs["Silvacis"]);  // Silvacis is unique, no clone needed

            Room nearFallenTree = CreateRoom(42, "nearFallenTree", "A wooded path near a fallen tree", "A gigantic fallen tree blocks the path here. You would have some difficulty climbing over it, but you notice a break in the trunk just south of the path that you can squeeze through to continue along.");
            nearFallenTree.Exits.Add("west", 41);
            nearFallenTree.Exits.Add("east", 43);

            Room pathWithCart = CreateRoom(43, "pathWithCart", "A wooded path with a broken cart", "As you enter this section of the wooded path, you notice a large wooden cart in a ditch on the side of the road.  The wheels are broken and the old wood is mossy and cracked.  It looks as though it's been here for ages.");
            pathWithCart.Exits.Add("west", 42);
            pathWithCart.Exits.Add("east", 44);
            pathWithCart.Items.Add("cart");
       
            Room abandonedCampsite = CreateRoom(44, "abandonedCampsite", "An abandoned campsite", "You come across the charred remains of a long abandoned campsite.  You notice a charred wooden chest sticking out of one of the several ash piles.  The west leads out of the forest, and you can hear a stream to the south.");
            abandonedCampsite.Exits.Add("west", 43);
            abandonedCampsite.Exits.Add("south", 45);
           
            Room forestStream = CreateRoom(45, "forestStream", "A stream running through the forest", "You stand before a shallow bubbling stream that runs east to west through the forest.  A waterlogged stump is stubbornly rooted in the center of the stream, the current splitting around it.  You can smell a campfire to the north. To the south a barely worn path continues into the forest, which gets darker the further you go.");
            forestStream.Exits.Add("north", 44);
            forestStream.Exits.Add("south", 46);
            forestStream.Items.Add("stump");

            Room forestBend = CreateRoom(46, "forestBend", "A bend in the forest path", "The path through the forest curves from the north to the east.  The path is looking less worn here, and the canopy overhead is so thick that only a few thin beams of light shine through.  You hear rustling leaves all around you, as though the creatures of this forest are constantly on the move.  To the north you can hear a stream, and to the east the path continues into the deeper parts of the woods.");
            forestBend.Exits.Add("north", 45);
            forestBend.Exits.Add("east", 47);
            forestBend.NPCs.Add(npcs["Dire Wolf"].Clone());  // Clone dire wolf
            forestBend.NPCs.Add(npcs["Dire Wolf"].Clone());  // Clone dire wolf
            forestBend.CanRespawn = true;
            forestBend.RespawnTimeHours = 16f;
            forestBend.OriginalNPCs.Add(npcs["Dire Wolf"].Clone());
            forestBend.OriginalNPCs.Add(npcs["Dire Wolf"].Clone());


            Room deepForest = CreateRoom(47, "deepForest", "A deep forest clearing", "This secluded clearing is barely visible from the path. A small campfire smolders near a worn bedroll, and weapons are carefully arranged on a fallen log. Someone has been living here. To the west the path turns back towards the forest entrance.  To the east the forest grows wildly.");
            
            deepForest.Exits.Add("west", 46);
            deepForest.Exits.Add("east", 48);
            deepForest.Items.Add("chest");
            deepForest.NPCs.Add(npcs["Braxus"]);  // Braxus is unique, no clone needed

            // ============================================
            // FOREST EXTENSION (Rooms 48-62)
            // The "maze-like" 6x3 forest grid
            // ============================================

            // ALL OF THIS NEEDS UPDATED UNLESS MARKED OTHERWISE

            // Continue east from abandonedCampsite (47)
            Room darkHollow = CreateRoom(48, "darkHollow", "A dark hollow", "The forest floor dips into a shadowy hollow here. Thick roots twist across the ground, and the air feels damp and still. The path bends here, leading both west and north.");
            darkHollow.Exits.Add("north", 49);
            darkHollow.Exits.Add("west", 47);

            Room tangledThicket = CreateRoom(49, "tangledThicket", "A tangled thicket", "Dense briars and thorny bushes crowd the path here. You thought you heard something to the north, but it was probably nothing.  Wait, there it was again! It's not too late to turn back.");
            tangledThicket.Exits.Add("north", 50);
            tangledThicket.Exits.Add("south", 48);

            Room mossyClearingNorth = CreateRoom(50, "mossyClearingNorth", "A mossy clearing", "Soft green moss carpets this small clearing. Shafts of light filter through gaps in the canopy above. The forest feels slightly less oppressive here.");
            mossyClearingNorth.Exits.Add("east", 51);
            mossyClearingNorth.Exits.Add("south", 49);

            Room ancientOak = CreateRoom(51, "ancientOak", "An ancient oak", "A massive oak tree dominates this area, its trunk wider than three men standing together. Strange symbols are carved into its bark, worn smooth by time.");
            ancientOak.Exits.Add("east", 52);
            ancientOak.Exits.Add("west", 50);
            

            Room overgrownRuins = CreateRoom(52, "overgrownRuins", "Overgrown ruins", "Crumbling stone walls peek through the undergrowth here - the remains of some long-forgotten structure. Vines have reclaimed most of it.");
            overgrownRuins.Exits.Add("west", 51);
            overgrownRuins.Exits.Add("south", 53);

            Room foggyPath = CreateRoom(53, "foggyPath", "A foggy clearing", "An unnatural fog blankets this clearing, thick and oppressive. Unlike normal mist, this fog seems almost alive - it swirls in patterns that defy the wind and pulses with a faint, sickly luminescence. The fog is so dense to the east that you cannot see more than a few feet in that direction. To the south, the mist thins slightly, revealing a path.");
            foggyPath.Exits.Add("south", 54);
            foggyPath.Exits.Add("north", 52);  // Back to overgrown ruins
            foggyPath.PuzzleId = "foggy_clearing_puzzle";

            Room wildernessTrail = CreateRoom(54, "wildernessTrail", "A wilderness trail", "A narrow trail winds through dense underbrush. Animal tracks crisscross the path - some disturbingly large. The fog continues to obscure the path. Are you sure you're heading in the right direction?");
            wildernessTrail.Exits.Add("north", 55);

            Room fungalGrove = CreateRoom(55, "fungalGrove", "A fungal grove", "Enormous mushrooms grow in clusters here, some taller than a man. Their caps glow faintly with an eerie bioluminescence. The air smells earthy and strange, but the fog has subsided here. You can make out paths to the east and to the west.");
            fungalGrove.Exits.Add("west", 56);
            fungalGrove.Exits.Add("east", 54);

            Room mossyClearingSouth = CreateRoom(56, "mossyClearingSouth", "A mossy clearing", "Another mossy clearing, similar to the one you may have seen to the north. Are you going in circles? The forest all looks the same here.");
            mossyClearingSouth.Exits.Add("north", 55);
            mossyClearingSouth.Exits.Add("south", 57);

            Room twistingPath = CreateRoom(57, "twistingPath", "A twisting path", "The path twists and turns here, making it difficult to maintain your bearings. Dense undergrowth surrounds you on all sides.");
            twistingPath.PuzzleId = "twisting_path_puzzle";
            // Exits hidden - revealed by puzzle interactions
            // twistingPath.Exits.Add("north", 56);  // Revealed by examining bootprints and moving vines
            // twistingPath.Exits.Add("west", 58);   // Revealed by examining bones/tracks and pushing branches

            twistingPath.Objects.Add(new RoomObject
            {
                Id = "animal_bones",
                Name = "bones",
                Aliases = new[] { "animal bones", "skeleton", "remains", "deer bones" },
                DefaultDescription = "Scattered bones lie half-buried in the undergrowth. They look like they belonged to a deer.",
                LookedAtDescription = "The bones have been picked clean. Whatever killed this deer dragged it through here - you can see faint drag marks and wolf tracks leading west through the undergrowth.",
                IsInteractable = false
            });

            twistingPath.Objects.Add(new RoomObject
            {
                Id = "wolf_tracks",
                Name = "tracks",
                Aliases = new[] { "wolf tracks", "prints", "paw prints", "pawprints" },
                DefaultDescription = "Fresh wolf tracks crisscross the area. They seem to converge from multiple directions.",
                LookedAtDescription = "Following the tracks with your eyes, you notice they all lead toward a particularly thick tangle of branches to the west. Something regularly passes through there.",
                IsInteractable = false,
                IsHidden = true  // Revealed after examining bones
            });

            twistingPath.Objects.Add(new RoomObject
            {
                Id = "thick_branches",
                Name = "branches",
                Aliases = new[] { "thick branches", "tangle", "undergrowth", "brush" },
                DefaultDescription = "A wall of tangled branches blocks any passage to the west.",
                LookedAtDescription = "Looking closer, the branches are less solid than they first appeared. After looking closely at these branches, you notice that a narrow path continues beyond them. Maybe you could push them out of the way?",
                IsInteractable = true,
                InteractionType = "push",
                OnInteractMessage = null,  // Handled by puzzle logic
                IsHidden = true  // Revealed after examining tracks
            });

            twistingPath.Objects.Add(new RoomObject
            {
                Id = "boot_prints",
                Name = "bootprints",
                Aliases = new[] { "boot prints", "footprints", "prints", "boots" },
                DefaultDescription = "Fresh bootprints mark the ground here.",
                LookedAtDescription = "Wait... these bootprints match your own boots exactly. You've been walking in circles! But looking more carefully, you can see your original tracks came from the north, where thick vines now conceal the path.",
                IsInteractable = false
            });

            twistingPath.Objects.Add(new RoomObject
            {
                Id = "thick_vines",
                Name = "vines",
                Aliases = new[] { "thick vines", "ivy", "growth" },
                DefaultDescription = "Thick vines hang across what might once have been a path to the north.",
                LookedAtDescription = "The vines are draped over the opening like a curtain. After looking closely at these vines, you notice that a path continues beyond them. You could probably move them aside to pass through.",
                IsInteractable = true,
                InteractionType = "move",
                OnInteractMessage = null,  // Handled by puzzle logic
                IsHidden = true  // Revealed after examining bootprints
            });

            // Needs update
            Room forestEdge = CreateRoom(58, "forestEdge", "The forest's edge", "The trees thin here and you can see open sky to the south. The forest seems reluctant to let you go, however, and thorny vines and branches reach out like grasping fingers to block your path.  The way you came from seems to have disappeared behind you.  Your only way forward is to the west.");
            forestEdge.Exits.Add("west", 59);

            // Needs update
            Room rottenLog = CreateRoom(59, "rottenLog", "A rotten log crossing", "A massive fallen log bridges a muddy depression here. The wood is soft and rotten - crossing it requires careful footing.");
            rottenLog.Exits.Add("north", 60);
            rottenLog.Exits.Add("east", 58);

            // Needs update
            Room wolfDen = CreateRoom(60, "wolfDen", "Near a wolf den", "You spot the dark entrance of a den dug into a hillside. Bones are scattered around the entrance. Something lives here - something hungry.");
            wolfDen.Exits.Add("north", 61);
            wolfDen.Exits.Add("south", 59);
            wolfDen.NPCs.Add(npcs["Dire Wolf"].Clone());
            wolfDen.NPCs.Add(npcs["Dire Wolf"].Clone());
            wolfDen.CanRespawn = true;
            wolfDen.RespawnTimeHours = 12f;
            wolfDen.OriginalNPCs.Add(npcs["Dire Wolf"].Clone());
            wolfDen.OriginalNPCs.Add(npcs["Dire Wolf"].Clone());


            // Needs update
            Room forestExit = CreateRoom(61, "forestExit", "Eastern forest exit", "The trees finally give way to open grassland. The dark forest looms behind you to the west. A worn path leads south toward... somewhere. Fresh air has never felt so good.");
            forestExit.Exits.Add("north", 48);
            forestExit.Exits.Add("south", 60);
    
            // ============================================
            // NORTH ROAD TO BELUM (Rooms 68-69)
            // ============================================

            // Needs update
            Room northRoad = CreateRoom(68, "northRoad", "The North Road", "The road north from the crossroads is well-maintained and shows signs of regular traffic. Wagon ruts line either side, and you can see the walls of a town in the distance. The crossroads lies to the south.");
            northRoad.Exits.Add("north", 69);
            northRoad.Exits.Add("south", 7);
            

            // Needs update
            Room belumApproach = CreateRoom(69, "belumApproach", "Approach to Belum", "The town walls of Belum rise before you, built of weathered grey stone. Guards patrol the battlements above. The main gate stands closed. A veteran guard named Marcus stands watch. A signpost reads: 'GATE CLOSED'.");
            belumApproach.Exits.Add("south", 68);
            belumApproach.NPCs.Add(npcs["Marcus"].Clone());  // Gate guard quest giver
            // belumApproach.Exits.Add("north", 70);  // Opens after completing bandit quest

            // BELUM - THE TOWN (Rooms 70-89)

            // Needs update
            Room belumSouthGate = CreateRoom(70, "belumSouthGate", "Belum - South Gate", "You pass through the southern gate of Belum. Guards in bronze armor eye travelers but make no move to stop you. The cobblestone streets are busy with merchants, locals, and fellow travelers. The main road continues north into the town center.");
            belumSouthGate.Exits.Add("north", 79);
            belumSouthGate.Exits.Add("east", 71);
            belumSouthGate.Exits.Add("south", 69);
            belumSouthGate.Exits.Add("west", 85);
            belumSouthGate.NPCs.Add(npcs["Town Guard"].Clone());

            // Needs update
            Room southMarket = CreateRoom(71, "southMarket", "Belum - South Market", "Stalls and carts line this section of the street, merchants hawking their wares. The smell of spices, leather, and fresh bread fills the air. The noise of commerce is constant.");
            southMarket.Exits.Add("north", 78);
            southMarket.Exits.Add("east", 72);
            southMarket.Exits.Add("west", 70);
            southMarket.NPCs.Add(npcs["Apothecary"].Clone());

            // Needs update
            Room stablesDistrict = CreateRoom(72, "stablesDistrict", "Belum - Stables District", "The smell of hay and horses is strong here. Several stables and paddocks house animals for travelers and merchants. A weathered sign advertises boarding rates.");
            stablesDistrict.Exits.Add("north", 73);
            stablesDistrict.Exits.Add("west", 71);
            

            // Needs update
            Room poorQuarter = CreateRoom(73, "poorQuarter", "Belum - Poor Quarter", "The buildings here are older and less maintained than elsewhere in town. Laundry hangs from windows, and children play in the narrow streets. The locals eye you with a mix of curiosity and wariness.");
            poorQuarter.Exits.Add("north", 74);
            poorQuarter.Exits.Add("south", 72);
            poorQuarter.Exits.Add("west", 78);

            // Needs update
            Room theGoldenGrape = CreateRoom(74, "theGoldenGrape", "Belum - The Golden Grape Tavern", "A large and welcoming tavern. The sign shows a golden bunch of grapes. Laughter and music spill out through the open door. This seems like the place to hear local news and rumors.");
            theGoldenGrape.Exits.Add("north", 75);
            theGoldenGrape.Exits.Add("south", 73);
            theGoldenGrape.Exits.Add("west", 77);
            theGoldenGrape.NPCs.Add(npcs["Barkeep"].Clone());

            // Needs update
            Room merchantRow = CreateRoom(75, "merchantRow", "Belum - Merchant Row", "Prosperous shops display their goods through glass windows - a luxury in these parts. Tailors, jewelers, and specialty craftsmen ply their trades here.");
            merchantRow.Exits.Add("west", 76);
            merchantRow.Exits.Add("south", 74);
            merchantRow.NPCs.Add(npcs["Scribe"].Clone());

            // Needs update
            Room craftsmansWay = CreateRoom(76, "craftsmansWay", "Belum - Craftsman's Way", "The ring of hammers on anvils echoes here. Blacksmiths, coopers, and carpenters work their trades. The air is warm from forge fires.");
            craftsmansWay.Exits.Add("west", 81);
            craftsmansWay.Exits.Add("south", 77);
            craftsmansWay.Exits.Add("east", 75);

            // Needs update
            Room backAlleys = CreateRoom(77, "backAlleys", "Belum - Back Alleys", "Narrow alleyways wind between cramped buildings. It's darker here, and the main bustle of the town feels far away. Not the safest part of town after dark.");
            backAlleys.Exits.Add("north", 76);
            backAlleys.Exits.Add("east", 74);
            backAlleys.Exits.Add("south", 78);
            backAlleys.Exits.Add("west", 80);

            // Needs update
            Room blacksmithForge = CreateRoom(78, "blacksmithForge", "Belum - The Iron Anvil", "A large smithy with multiple forges burning hot. Weapons, tools, and armor line the walls. The smith is a mountain of a man with arms like tree trunks.");
            blacksmithForge.Exits.Add("north", 77);
            blacksmithForge.Exits.Add("south", 71);
            blacksmithForge.Exits.Add("west", 79);
            blacksmithForge.NPCs.Add(npcs["Blacksmith"].Clone());

            // Needs update
            Room mainStreetSouth = CreateRoom(79, "mainStreetSouth", "Belum - Main Street (South)", "The main thoroughfare of Belum stretches north and south. Shops and taverns line both sides. A public fountain provides a gathering spot for locals.");
            mainStreetSouth.Exits.Add("north", 80);
            mainStreetSouth.Exits.Add("east", 78);
            mainStreetSouth.Exits.Add("south", 70);
            mainStreetSouth.Exits.Add("west", 84);
            mainStreetSouth.NPCs.Add(npcs["Villager"].Clone());

            // Needs update
            Room townSquare = CreateRoom(80, "townSquare", "Belum - Town Square", "The heart of Belum opens into a grand square. A large stone fountain depicting Marea, goddess of the sea, dominates the center. Important-looking buildings surround the square - the town hall, a temple, and what appears to be a guild house of some kind.");
            townSquare.Exits.Add("north", 81);
            townSquare.Exits.Add("east", 77);
            townSquare.Exits.Add("south", 79);
            townSquare.Exits.Add("west", 83);
            townSquare.NPCs.Add(npcs["Town Guard"].Clone());
            townSquare.NPCs.Add(npcs["Villager"].Clone());
            townSquare.NPCs.Add(npcs["Merchant"].Clone());

            Room residentialNorth = CreateRoom(81, "residentialNorth", "Belum - North Residential", "Well-appointed homes belonging to Belum's more prosperous citizens line this quiet street. Gardens and small courtyards provide greenery.  The northern gate leading out of town is to the north, shops are to the east, the town square lies directly south, and to the west is a large plaza surrounded by temples to the gods.");
            residentialNorth.Exits.Add("north",90);
            residentialNorth.Exits.Add("east", 76);
            residentialNorth.Exits.Add("south", 80);
            residentialNorth.Exits.Add("west", 82);

            Room templeDistrict = CreateRoom(82, "templeDistrict", "Belum - Temple District", "Massive temples of white stone surround an open plaza.  The largest of all of these, to the north, is devoted to Keius - father of the gods.  If you look straight up, you can barely see the tops of the colossal columns that line the temple's facade.  Temples to major and minor gods of Keius' pantheon fill the rest of the square.  To the east is a residential district on the main street through Belum.  To the south are several inns, and more residences to the west.");
            templeDistrict.Exits.Add("east", 81);
            templeDistrict.Exits.Add("south", 83);
            templeDistrict.Exits.Add("west", 89);

            // Needs update
            Room innDistrict = CreateRoom(83, "innDistrict", "Belum - Inn District", "Several inns compete for business here, their signs creaking in the breeze. 'The Wanderer's Rest', 'The Sleeping Lion', and 'Beds & Breakfast' all promise comfortable lodging.");
            innDistrict.Exits.Add("north", 82);
            innDistrict.Exits.Add("east", 80);
            innDistrict.Exits.Add("south", 84);
            innDistrict.Exits.Add("west", 88);

            // Needs update
            Room armorersRow = CreateRoom(84, "armorersRow", "Belum - Armorer's Row", "Shops specializing in armor and protective gear line this street. Mannequins display everything from leather jerkins to full plate mail. A testing dummy stands outside one shop, heavily dented.");
            armorersRow.Exits.Add("north", 83);
            armorersRow.Exits.Add("east", 79);
            armorersRow.Exits.Add("south", 85);
            armorersRow.Exits.Add("west", 87);

            // Needs update
            Room thievesGuild = CreateRoom(85, "thievesGuild", "Belum - Unmarked Alley", "A dead-end alley with a single unmarked door. Those who know, know. Those who don't, shouldn't be here.");
            thievesGuild.Exits.Add("north", 84);
            thievesGuild.Exits.Add("east", 70);
            thievesGuild.Exits.Add("west", 86);

            // Needs update
            Room shadowyCorner = CreateRoom(86, "shadowyCorner", "Belum - A Shadowy Corner", "This corner of town sees less traffic. A nondescript door leads to what might be a less-than-legitimate establishment. The locals here don't meet your eyes.");
            shadowyCorner.Exits.Add("north", 87);
            shadowyCorner.Exits.Add("east", 85);

            Room townHall = CreateRoom(87, "townHall", "Belum - Town Hall", "The administrative center of Belum. An imposing classical building with tall marble columns and bronze doors stands before you. Guards stand at attention near the entrance. A notice board displays official proclamations. The bronze doors to the west lead inside the building.");
            townHall.Exits.Add("north", 88);
            townHall.Exits.Add("south", 86);
            townHall.Exits.Add("east", 84);
            townHall.Exits.Add("west", 91);
            townHall.NPCs.Add(npcs["Town Guard"].Clone());

            // Needs update
            Room barracks = CreateRoom(88, "barracks", "Belum - Guard Barracks", "The town guard's headquarters. Soldiers drill in a courtyard while others sharpen weapons or play dice. The captain's office is visible through an open door.");
            barracks.Exits.Add("east", 83);
            barracks.Exits.Add("south", 87);
            barracks.NPCs.Add(npcs["Town Guard"].Clone());
            barracks.NPCs.Add(npcs["Town Guard"].Clone());

            Room templeOfKeius = CreateRoom(89, "templeOfKeius", "Belum - Temple of Keius", "You step into the Temple of Keius and immediately feel small. The columns soar far above the city walls that surround Belum. The ceiling is lost in shadow. Everything here is built on a scale meant to humble mortals before the divine. To complete that feeling, an enormous statue of Keius sits at the back of the temple, looking onward in judgement of all who enter.  Back to the east lies the plaza of the Temple District.");
            templeOfKeius.Exits.Add("east", 82);
            templeOfKeius.NPCs.Add(npcs["Caelia"]);

            Room belumNorthGate = CreateRoom(90, "belumNorthGate", "Belum - North Gate", "A large wooden and iron gate stands open here, stone towers on either side. Armed guards stand at attention, watching for any sign of trouble.  To the south is the residential district, and the road north leads out of town.");
            belumNorthGate.Exits.Add("south", 81);

            Room townHallInterior = CreateRoom(91, "townHallInterior", "Belum - Town Hall Interior", "You step into the interior of an ancient classical administrative building. Tall marble columns rise to support a vaulted ceiling decorated with frescoes depicting the founding of Belum. Rows of wooden benches face a raised platform where officials conduct town business. Scrolls, ledgers, and official documents are neatly organized on shelves along the walls. The air smells of parchment and wax seals. To the east is the exit back to the town square.");
            townHallInterior.Exits.Add("east", 87);
            townHallInterior.NPCs.Add(npcs["Senator Quintus"]);

            // ===== CULTIST HIDEOUT (100-120) - Forest hideout of Ordo Dissolutus =====

            // Room 100 - Hideout Entrance (accessed via fog puzzle in Room 53)
            Room hideoutEntrance = CreateRoom(100, "hideoutEntrance", "Cultist Hideout - Entrance", "Beyond the dissipated fog, a hidden path leads to a clearing dominated by a crude wooden structure built against the base of an enormous hollow tree. The bark has been carved with disturbing symbols - spirals that seem to unravel the longer you look at them, and texts that fade and blur as if rejecting coherence. The entrance yawns open like a wound. To the west, the fog-cleared path leads back to the forest. To the east, darkness beckons.");
            hideoutEntrance.Exits.Add("west", 53);
            hideoutEntrance.Exits.Add("east", 101);

            // Room 101 - Guard Post
            Room guardPost = CreateRoom(101, "guardPost", "Cultist Hideout - Guard Post", "This chamber serves as a watchtower for the cultists. Simple wooden furniture has been deliberately broken and left in disarray - not from violence, but as a statement. A lookout post faces the entrance. Cultist guards patrol here, their eyes cold and fanatical. Passages lead east and south deeper into the hideout.");
            guardPost.Exits.Add("west", 100);
            guardPost.Exits.Add("east", 102);
            guardPost.Exits.Add("south", 105);
            guardPost.NPCs.Add(npcs["Cultist Scout"].Clone());
            guardPost.NPCs.Add(npcs["Cultist Zealot"].Clone());

            // Room 102 - Supply Cache
            Room supplyCache = CreateRoom(102, "supplyCache", "Cultist Hideout - Supply Cache", "Crates and barrels are stacked here, though many have been deliberately damaged - grain spilled, water barrels cracked open. The cultists seem to take only what they need and destroy the rest. A few intact containers remain that you could loot. The only exit is back west.");
            supplyCache.Exits.Add("west", 101);
            supplyCache.Items.Add("greater potion");
            supplyCache.Items.Add("energy potion");

            // Room 105 - Ritual Chamber (cipher puzzle)
            Room ritualChamber = CreateRoom(105, "ritualChamber", "Cultist Hideout - Ritual Chamber", "A circular room with symbols painted on the floor in ash and charcoal. The air feels wrong here, as if reality itself is slightly unstable. Six stone pedestals ring the chamber, each bearing a carved symbol. In the center, a locked iron gate blocks passage to the south - but there's no visible keyhole, only a phrase carved above: 'Speak the word that unmakes all things.' The symbols on the pedestals seem to be parts of a puzzle.");
            ritualChamber.Exits.Add("north", 101);
            ritualChamber.PuzzleId = "ritual_chamber_cipher";

            ritualChamber.Objects.Add(new RoomObject
            {
                Id = "stone_pedestals",
                Name = "pedestals",
                Aliases = new[] { "stone pedestals", "pedestal", "symbols", "carved symbols" },
                DefaultDescription = "Six stone pedestals arranged in a circle, each carved with a different symbol.",
                LookedAtDescription = "Each pedestal bears a symbol and a word beneath it:\n1. A flame - 'IGNIS' (Fire)\n2. A droplet - 'AQUA' (Water)\n3. A skull - 'MORTIS' (Death)\n4. A spiral - 'VORAGO' (Abyss)\n5. A shattered circle - 'FRACTUS' (Broken)\n6. An empty void - 'NIHIL' (Nothing)\n\nThe phrase above the gate reads: 'Speak the word that unmakes all things.' What could it mean?",
                IsInteractable = false
            });

            ritualChamber.Objects.Add(new RoomObject
            {
                Id = "iron_gate",
                Name = "gate",
                Aliases = new[] { "iron gate", "locked gate", "southern gate" },
                DefaultDescription = "A heavy iron gate blocks the southern passage. Above it is carved: 'Speak the word that unmakes all things.'",
                LookedAtDescription = "The gate has no lock or handle - only the carved phrase. It seems to require a spoken answer to open.",
                IsInteractable = false
            });

            // Room 108 - Defaced Library (book puzzle)
            Room defacedLibrary = CreateRoom(108, "defacedLibrary", "Cultist Hideout - Defaced Library", "What was once a repository of knowledge has been systematically destroyed. Bookshelves line the walls, but most books have been torn apart, their pages scattered like snow. Some books have been burned, others unraveled thread by thread. Yet a few volumes remain intact on a central reading table - perhaps they contain something the cultists found useful. A passage leads north, and a narrow hallway continues east.");
            defacedLibrary.Exits.Add("north", 105);
            defacedLibrary.Exits.Add("east", 110);
            defacedLibrary.PuzzleId = "library_book_puzzle";

            defacedLibrary.Objects.Add(new RoomObject
            {
                Id = "reading_table",
                Name = "table",
                Aliases = new[] { "reading table", "central table", "books", "intact books" },
                DefaultDescription = "A reading table with several intact books that survived the destruction.",
                LookedAtDescription = "Three books remain intact:\n\n'Principles of Order' - A philosophical text. The cultists have annotated it heavily with contemptuous notes in the margins: 'All order is temporary,' 'Entropy claims all,' 'Why preserve what will inevitably fall?'\n\n'The Founding of Aevoria' - A historical text. One passage is circled in red: 'The five seals were placed to contain the ancient chaos. Should they fail, the empire itself would unravel.'\n\n'Rituals of Unbinding' - A dangerous tome. Instructions for breaking magical seals and disrupting protective wards. A bookmark marks a page titled 'The Festival Convergence - When barriers are weakest.'",
                IsInteractable = false
            });

            defacedLibrary.Objects.Add(new RoomObject
            {
                Id = "scattered_pages",
                Name = "pages",
                Aliases = new[] { "scattered pages", "torn pages", "paper", "destroyed books" },
                DefaultDescription = "Thousands of torn pages cover the floor like autumn leaves.",
                LookedAtDescription = "Most are illegible, but you spot a few fragments:\n'...the old empire knew what we have forgotten...'\n'...containing rather than destroying was their mistake...'\n'...when enough seals break, the cascade cannot be stopped...'\n\nThe destruction here was methodical and purposeful.",
                IsInteractable = false
            });

            // Room 110 - Monument of Unmaking (environmental puzzle)
            Room monumentRoom = CreateRoom(110, "monumentRoom", "Cultist Hideout - Monument of Unmaking", "A large chamber dominated by what was once a beautiful marble statue of an ancient emperor. The cultists have defaced it systematically - chipping away features, carving spirals into the stone, covering it in ash. Around the statue, offerings have been placed: broken tools, shredded documents, snapped weapons. This is a shrine to destruction itself. Passages lead west back to the library and south deeper into the complex.");
            monumentRoom.Exits.Add("west", 108);
            monumentRoom.Exits.Add("south", 115);
            monumentRoom.NPCs.Add(npcs["Cultist Defacer"].Clone());
            monumentRoom.NPCs.Add(npcs["Cultist Philosopher"].Clone());

            monumentRoom.Objects.Add(new RoomObject
            {
                Id = "defaced_statue",
                Name = "statue",
                Aliases = new[] { "marble statue", "emperor statue", "defaced statue", "monument" },
                DefaultDescription = "A marble statue of an ancient emperor, systematically defaced and covered in disturbing symbols.",
                LookedAtDescription = "The statue once depicted Emperor Valerius the First, founder of the empire. The cultists have carved away his face, replaced his scepter with a spiral symbol, and inscribed across the base: 'Even stone crumbles. Even empires fall. We hasten the inevitable.' The workmanship of the defacement is almost artistic in its precision - this wasn't vandalism, but ritual.",
                IsInteractable = false
            });

            monumentRoom.Objects.Add(new RoomObject
            {
                Id = "offerings",
                Name = "offerings",
                Aliases = new[] { "broken tools", "shredded documents", "offerings", "shrine" },
                DefaultDescription = "Various destroyed objects arranged around the statue like offerings at a shrine.",
                LookedAtDescription = "Broken hammers, split anvils, torn maps, shredded legal documents, snapped swords - each carefully placed. You recognize some of the documents: property deeds, marriage certificates, trade agreements. The cultists are collecting symbols of order and civilization to ritually destroy them. It's deeply unsettling.",
                IsInteractable = false
            });

            // Room 115 - Prison Cells (Althea's location)
            Room prisonCells = CreateRoom(115, "prisonCells", "Cultist Hideout - Prison Cells", "A row of crude iron cages lines the wall of this chamber. Most are empty, their doors hanging open, but one cell in the far corner is locked. Inside, you can see a figure huddled in the shadows - a young woman in tattered robes, her eyes glowing faintly with an eerie light even in the darkness. She looks up as you enter, hope flickering across her gaunt face. Passages lead north and east.");
            prisonCells.Exits.Add("north", 110);
            prisonCells.Exits.Add("east", 118);
            prisonCells.NPCs.Add(npcs["Cultist Lieutenant"].Clone());
            prisonCells.NPCs.Add(npcs["Althea"]);  // Doesn't clone - she's unique

            prisonCells.Objects.Add(new RoomObject
            {
                Id = "prison_cells",
                Name = "cells",
                Aliases = new[] { "cages", "iron cages", "prison", "locked cell" },
                DefaultDescription = "A row of iron cages. Most are empty, but one in the corner is locked with a figure inside.",
                LookedAtDescription = "The cells are crudely constructed but effective. The locked cell contains a young woman who watches you with a mixture of hope and suspicion. She appears weak but alert. A key ring hangs on the wall nearby - the lieutenant probably carries the actual key.",
                IsInteractable = false
            });

            // Room 118 - Antechamber
            Room antechamber = CreateRoom(118, "antechamber", "Cultist Hideout - Antechamber", "This chamber serves as a waiting area before the inner sanctum. The walls are covered in elaborate murals depicting the fall of civilizations - towers crumbling, books burning, monuments sinking into the earth. The artistry is disturbing in its beauty. To the west are the prison cells, and to the east, heavy double doors lead to what must be the leader's chamber. Cultist guards stand watch.");
            antechamber.Exits.Add("west", 115);
            antechamber.Exits.Add("east", 120);
            antechamber.NPCs.Add(npcs["Cultist Breaker"].Clone());
            antechamber.NPCs.Add(npcs["Cultist Archivist"].Clone());

            antechamber.Objects.Add(new RoomObject
            {
                Id = "murals",
                Name = "murals",
                Aliases = new[] { "wall murals", "paintings", "artwork", "walls" },
                DefaultDescription = "Elaborate murals covering the walls, depicting the fall of civilizations.",
                LookedAtDescription = "The murals are masterfully painted, showing:\n- The Library of Alexandria burning, scholars weeping\n- The Tower of Babel collapsing as people scatter\n- Great monuments being swallowed by sand and time\n- Cities crumbling as nature reclaims them\n\nAt the bottom of each scene is written: 'Ordo Dissolutus' - the Dissolved Order. These cultists don't just destroy - they philosophically embrace entropy itself.",
                IsInteractable = false
            });

            antechamber.Objects.Add(new RoomObject
            {
                Id = "double_doors",
                Name = "doors",
                Aliases = new[] { "heavy doors", "double doors", "eastern doors", "sanctum doors" },
                DefaultDescription = "Heavy wooden doors reinforced with iron bands, leading east to the inner sanctum.",
                LookedAtDescription = "The doors are carved with a massive spiral symbol that seems to draw your eye inward. Above them, an inscription reads: 'Beyond this threshold, the Archon of Unmaking awaits. Enter and witness the inevitable.'",
                IsInteractable = false
            });

            // Room 120 - Boss Chamber (Archon Malachar)
            Room archonChamber = CreateRoom(120, "archonChamber", "Cultist Hideout - The Archon's Sanctum", "A vast circular chamber at the heart of the hideout. The walls are carved with spiraling symbols that seem to move in the corner of your vision. In the center, ritual circles have been drawn in ash and blood, and the air itself seems to shimmer with wrongness. This is clearly the cult's inner sanctum - a place of dark rituals and forbidden practices.");
            archonChamber.Exits.Add("west", 118);
            archonChamber.NPCs.Add(npcs["Archon Malachar"].Clone());
            archonChamber.Items.Add("ritual dagger");
            archonChamber.Items.Add("cultist orders");
            archonChamber.Items.Add("ritual notes");
            archonChamber.Items.Add("philosophical tract");
            archonChamber.Items.Add("cell key");

            archonChamber.Objects.Add(new RoomObject
            {
                Id = "ritual_circles",
                Name = "circles",
                Aliases = new[] { "ritual circles", "ash circles", "blood circles", "symbols" },
                DefaultDescription = "Complex ritual circles drawn on the floor in ash and blood.",
                LookedAtDescription = "The circles are incredibly intricate, with symbols you don't recognize interwoven with mathematical formulae and arcane sigils. At specific points around the circles, small offerings have been placed - broken seals, torn documents, shattered amulets. This ritual appears designed to break or weaken something, though you can't tell what.",
                IsInteractable = false
            });

            archonChamber.Objects.Add(new RoomObject
            {
                Id = "archon_throne",
                Name = "throne",
                Aliases = new[] { "chair", "seat", "throne" },
                DefaultDescription = "A simple stone chair at the far end of the chamber, deliberately plain and unadorned.",
                LookedAtDescription = "Unlike the elaborate thrones of rulers, this is just a plain stone seat. The message is clear: the Archon has no need for the trappings of power or authority. He sees himself as merely an instrument of entropy, not a king. Somehow, that makes him more dangerous.",
                IsInteractable = false
            });

            // Guild Hall
            rooms.Add(1, bedroom);
            rooms.Add(2, hallway);
            rooms.Add(3, study);
            rooms.Add(4, commonArea);
            rooms.Add(5, frontDoor);

            // Progressive Guild Hall Rooms (unlock based on recruit count)
            rooms.Add(64, trainingYard);
            rooms.Add(65, armory);
            rooms.Add(66, treasury);
            rooms.Add(67, portalRoom);

            // Crossroads Area
            rooms.Add(6, guildPath);
            rooms.Add(7, theCrossRoads);
            rooms.Add(8, westPath);
            rooms.Add(9, westernBend);

            // Gaius' Farm
            rooms.Add(10, gaiusFarmFields);
            rooms.Add(11, gaiusFarmHouse);

            // Mountain Lower Slopes
            rooms.Add(12, lowerSlopes);

            // Bandit Caves
            rooms.Add(13, caveEntrance);
            rooms.Add(14, caveTunnel);
            rooms.Add(15, westernBranch);
            rooms.Add(16, westernChamber);
            rooms.Add(17, darkPassage);
            rooms.Add(18, deepCave);
            rooms.Add(19, floodedChamber);
            rooms.Add(20, undergroundRiver);
            rooms.Add(21, warlordChamber);


            rooms.Add(22, mountainPath);
            rooms.Add(23, rockyOutcrop);
            rooms.Add(24, iceCavern);
            rooms.Add(25, mountainPeak);
            rooms.Add(26, ancientAltar);

            // Forest
            rooms.Add(40, eastPath);
            rooms.Add(41, woodedPath);
            rooms.Add(42, nearFallenTree);
            rooms.Add(43, pathWithCart);
            rooms.Add(44, abandonedCampsite);
            rooms.Add(45, forestStream);
            rooms.Add(46, forestBend);
            rooms.Add(47, deepForest);
            rooms.Add(48, darkHollow);
            rooms.Add(49, tangledThicket);
            rooms.Add(50, mossyClearingNorth);
            rooms.Add(51, ancientOak);
            rooms.Add(52, overgrownRuins);
            rooms.Add(53, foggyPath);
            rooms.Add(54, wildernessTrail);
            rooms.Add(55, fungalGrove);
            rooms.Add(56, mossyClearingSouth);
            rooms.Add(57, twistingPath);
            rooms.Add(58, forestEdge);
            rooms.Add(59, rottenLog);
            rooms.Add(60, wolfDen);
            rooms.Add(61, forestExit);

            // Crossroads to Belum
            rooms.Add(68, northRoad);
            rooms.Add(69, belumApproach);

            // Belum
            rooms.Add(70, belumSouthGate);
            rooms.Add(71, southMarket);
            rooms.Add(72, stablesDistrict);
            rooms.Add(73, poorQuarter);
            rooms.Add(74, theGoldenGrape);
            rooms.Add(75, merchantRow);
            rooms.Add(76, craftsmansWay);
            rooms.Add(77, backAlleys);
            rooms.Add(78, blacksmithForge);
            rooms.Add(79, mainStreetSouth);
            rooms.Add(80, townSquare);
            rooms.Add(81, residentialNorth);
            rooms.Add(82, templeDistrict);
            rooms.Add(83, innDistrict);
            rooms.Add(84, armorersRow);
            rooms.Add(85, thievesGuild);
            rooms.Add(86, shadowyCorner);
            rooms.Add(87, townHall);
            rooms.Add(88, barracks);
            rooms.Add(89, templeOfKeius);
            rooms.Add(90, belumNorthGate);
            rooms.Add(91, townHallInterior);

            // Cultist Hideout
            rooms.Add(100, hideoutEntrance);
            rooms.Add(101, guardPost);
            rooms.Add(102, supplyCache);
            rooms.Add(105, ritualChamber);
            rooms.Add(108, defacedLibrary);
            rooms.Add(110, monumentRoom);
            rooms.Add(115, prisonCells);
            rooms.Add(118, antechamber);
            rooms.Add(120, archonChamber);

            // ===== TESTING ROOMS (999-991) - Hidden recruit testing area =====
            // Test Room 999 - Valeria (Legionnaire, Female, Conversational)
            Room testRoom999 = CreateRoom(999, "testRoom999", "[TEST] Recruit Testing Area 1", "[TEST ROOM] A simple testing chamber. Use 'teleport 999' to get here.");
            testRoom999.Exits.Add("south", 998);
            testRoom999.NPCs.Add(npcs["Valeria"].Clone());

            // Test Room 998 - Darius (Venator, Male, Conversational)
            Room testRoom998 = CreateRoom(998, "testRoom998", "[TEST] Recruit Testing Area 2", "[TEST ROOM] Another testing chamber.");
            testRoom998.Exits.Add("north", 999);
            testRoom998.Exits.Add("south", 997);
            testRoom998.NPCs.Add(npcs["Darius"].Clone());

            // Test Room 997 - Lyra (Oracle, Female, Conversational)
            Room testRoom997 = CreateRoom(997, "testRoom997", "[TEST] Recruit Testing Area 3", "[TEST ROOM] Yet another testing chamber.");
            testRoom997.Exits.Add("north", 998);
            testRoom997.Exits.Add("south", 996);
            testRoom997.NPCs.Add(npcs["Lyra"].Clone());

            // Test Room 996 - Marcus the Bold (Legionnaire, Male, Combat)
            Room testRoom996 = CreateRoom(996, "testRoom996", "[TEST] Recruit Testing Area 4", "[TEST ROOM] Combat recruit testing - talk to Marcus to fight.");
            testRoom996.Exits.Add("north", 997);
            testRoom996.Exits.Add("south", 995);
            testRoom996.NPCs.Add(npcs["Marcus the Bold"].Clone());

            // Test Room 995 - Aria Swift (Venator, Female, Combat)
            Room testRoom995 = CreateRoom(995, "testRoom995", "[TEST] Recruit Testing Area 5", "[TEST ROOM] Combat recruit testing - talk to Aria to fight.");
            testRoom995.Exits.Add("north", 996);
            testRoom995.Exits.Add("south", 994);
            testRoom995.NPCs.Add(npcs["Aria Swift"].Clone());

            // Test Room 994 - Aldric the Wise (Oracle, Male, Combat)
            Room testRoom994 = CreateRoom(994, "testRoom994", "[TEST] Recruit Testing Area 6", "[TEST ROOM] Combat recruit testing - talk to Aldric to fight.");
            testRoom994.Exits.Add("north", 995);
            testRoom994.Exits.Add("south", 993);
            testRoom994.NPCs.Add(npcs["Aldric the Wise"].Clone());

            // Test Room 993 - Thora (Legionnaire, Female, Conversational)
            Room testRoom993 = CreateRoom(993, "testRoom993", "[TEST] Recruit Testing Area 7", "[TEST ROOM] More conversational testing.");
            testRoom993.Exits.Add("north", 994);
            testRoom993.Exits.Add("south", 992);
            testRoom993.NPCs.Add(npcs["Thora"].Clone());

            // Test Room 992 - Fenris (Venator, Male, Conversational)
            Room testRoom992 = CreateRoom(992, "testRoom992", "[TEST] Recruit Testing Area 8", "[TEST ROOM] Beast tracker testing.");
            testRoom992.Exits.Add("north", 993);
            testRoom992.Exits.Add("south", 991);
            testRoom992.NPCs.Add(npcs["Fenris"].Clone());

            // Test Room 991 - Celestia (Oracle, Female, Conversational)
            Room testRoom991 = CreateRoom(991, "testRoom991", "[TEST] Recruit Testing Area 9", "[TEST ROOM] Final testing chamber. Use 'teleport 1' to return to guild hall.");
            testRoom991.Exits.Add("north", 992);
            testRoom991.NPCs.Add(npcs["Celestia"].Clone());

            // Add test rooms
            rooms.Add(999, testRoom999);
            rooms.Add(998, testRoom998);
            rooms.Add(997, testRoom997);
            rooms.Add(996, testRoom996);
            rooms.Add(995, testRoom995);
            rooms.Add(994, testRoom994);
            rooms.Add(993, testRoom993);
            rooms.Add(992, testRoom992);
            rooms.Add(991, testRoom991);

            // ===== AEVORIA - IMPERIAL VILLA =====
            // Room 200: Imperial Villa - Main Hall (arrival point from guild council meeting)
            Room imperialVillaHall = CreateRoom(200, "imperialVillaHall", "Imperial Villa - Grand Hall",
                "You stand in the magnificent grand hall of the Emperor's private villa. Soaring marble columns support a vaulted ceiling painted with scenes of imperial conquest. Sunlight streams through tall windows, casting patterns across the polished floor. Rich tapestries depicting the Empire's 1500-year history line the walls.<br><br>Senator Quintus and High Priestess Caelia stand nearby, speaking in low tones. Imperial guards in ceremonial armor stand at attention throughout the hall.");
            imperialVillaHall.Exits.Add("north", 201);  // Throne Room
            imperialVillaHall.Exits.Add("east", 202);   // Guest Quarters
            imperialVillaHall.Exits.Add("south", 203);  // Gardens
            imperialVillaHall.Exits.Add("west", 204);   // Library

            // Room 201: Imperial Villa - Throne Room (Emperor is here)
            Room throneRoom = CreateRoom(201, "imperialVillaThroneRoom", "Imperial Villa - Throne Room",
                "An opulent throne room fit for the ruler of an empire. The throne itself is carved from a single piece of black marble, inlaid with gold depicting the imperial eagle. Banners of the legions that conquered the known world hang from the ceiling. Despite the grandeur, the room has an intimate quality - this is the Emperor's private audience chamber, not the vast public throne room of the palace.<br><br>Emperor Certius awaits here, ready to receive visitors.");
            throneRoom.Exits.Add("south", 200);  // Back to Grand Hall
            if (npcs.ContainsKey("Emperor Certius"))
            {
                throneRoom.NPCs.Add(npcs["Emperor Certius"].Clone());
            }

            // Room 202: Imperial Villa - Guest Quarters
            Room guestQuarters = CreateRoom(202, "imperialVillaGuestQuarters", "Imperial Villa - Guest Quarters",
                "Luxurious guest quarters have been prepared for you and your companions. The room features plush beds with silk sheets, a writing desk of polished mahogany, and a balcony overlooking the city. Fresh fruit and wine have been laid out on a side table.<br><br>A bronze plaque reads: 'Guests of the Emperor are granted the Empire's hospitality.'");
            guestQuarters.Exits.Add("west", 200);  // Back to Grand Hall

            // Room 203: Imperial Villa - Gardens
            Room villaGardens = CreateRoom(203, "imperialVillaGardens", "Imperial Villa - Gardens",
                "Meticulously maintained gardens surround a central fountain depicting the goddess Victoria. Rare flowers from every corner of the Empire bloom in carefully arranged beds. Stone paths wind between sculpted hedges, and the scent of jasmine fills the air. Servants tend the plants with quiet efficiency.<br><br>This is a place of peace, though the weight of impending crisis makes relaxation difficult.");
            villaGardens.Exits.Add("north", 200);  // Back to Grand Hall

            // Room 204: Imperial Villa - Library
            Room villaLibrary = CreateRoom(204, "imperialVillaLibrary", "Imperial Villa - Imperial Library",
                "Floor-to-ceiling shelves hold thousands of scrolls and bound volumes - the personal collection of Emperor Certius himself. You spot texts on military strategy, philosophy, history, and governance. A large table in the center holds maps of the Empire and its frontiers.<br><br>Several scholars work quietly at desks, cataloging and preserving this knowledge for future generations.");
            villaLibrary.Exits.Add("east", 200);  // Back to Grand Hall

            // Add Imperial Villa rooms
            rooms.Add(200, imperialVillaHall);
            rooms.Add(201, throneRoom);
            rooms.Add(202, guestQuarters);
            rooms.Add(203, villaGardens);
            rooms.Add(204, villaLibrary);

            // ===== AEVORIA - COLOSSEUM (Anniversary Celebration) =====
            // Room 220: Colosseum - Lower Gallery (entrance, first encounter)
            Room colosseumLowerGallery = CreateRoom(220, "colosseumLowerGallery", "Colosseum - Lower Gallery",
                "The lower gallery of the Colosseum is packed with spectators. Banners in imperial purple hang from the stone arches. The roar of the crowd echoes off the walls as gladiatorial games unfold in the arena below. Vendors hawk wine and food, and the air is thick with excitement.<br><br>Ahead, you notice several figures in hooded robes moving against the crowd flow - heading deeper into the structure. Their movements are too purposeful, too synchronized. Cultists.");
            colosseumLowerGallery.Exits.Add("north", 221);  // To mid gallery
            // Add 2 cultist scouts (level 6-7)
            if (npcs.ContainsKey("Cultist Scout"))
            {
                colosseumLowerGallery.NPCs.Add(npcs["Cultist Scout"].Clone());
                colosseumLowerGallery.NPCs.Add(npcs["Cultist Scout"].Clone());
            }

            // Room 221: Colosseum - Mid Gallery (second encounter)
            Room colosseumMidGallery = CreateRoom(221, "colosseumMidGallery", "Colosseum - Mid Gallery",
                "You've climbed to the mid-level gallery, where wealthier citizens watch the games from cushioned seats. The view of the arena is spectacular from here, but you have no time for spectacle. More cultists block your path - these ones are armored and ready for combat. They've abandoned all pretense of blending in.");
            colosseumMidGallery.Exits.Add("south", 220);  // Back to lower gallery
            colosseumMidGallery.Exits.Add("north", 222);  // To upper gallery
            // Add 3 mid-level cultists (zealot + defacer + scout)
            if (npcs.ContainsKey("Cultist Zealot") && npcs.ContainsKey("Cultist Defacer"))
            {
                colosseumMidGallery.NPCs.Add(npcs["Cultist Zealot"].Clone());
                colosseumMidGallery.NPCs.Add(npcs["Cultist Defacer"].Clone());
                colosseumMidGallery.NPCs.Add(npcs["Cultist Scout"].Clone());
            }

            // Room 222: Colosseum - Upper Gallery (third encounter)
            Room colosseumUpperGallery = CreateRoom(222, "colosseumUpperGallery", "Colosseum - Upper Gallery",
                "The upper gallery is reserved for senators and high nobility. The seats here are empty - the elite are all in private viewing boxes. You're close to the Emperor's box now. A group of cultists has taken position here as a final line of defense. These are veteran fighters arrayed in defensive formation, weapons drawn and ready.");
            colosseumUpperGallery.Exits.Add("south", 221);  // Back to mid gallery
            colosseumUpperGallery.Exits.Add("north", 223);  // To Emperor's seat box
            // Add 4 high-level cultists (lieutenant + philosopher + zealot + defacer)
            if (npcs.ContainsKey("Cultist Lieutenant") && npcs.ContainsKey("Cultist Philosopher") &&
                npcs.ContainsKey("Cultist Zealot") && npcs.ContainsKey("Cultist Defacer"))
            {
                colosseumUpperGallery.NPCs.Add(npcs["Cultist Lieutenant"].Clone());
                colosseumUpperGallery.NPCs.Add(npcs["Cultist Philosopher"].Clone());
                colosseumUpperGallery.NPCs.Add(npcs["Cultist Zealot"].Clone());
                colosseumUpperGallery.NPCs.Add(npcs["Cultist Defacer"].Clone());
            }

            // Room 223: Colosseum - Emperor's Seat Box (assassination scene)
            Room emperorSeatBox = CreateRoom(223, "emperorSeatBox", "Colosseum - Emperor's Seat Box",
                "The Emperor's private viewing box offers an unobstructed view of the arena floor. Rich purple tapestries bearing the imperial eagle frame the space. This is where rulers of the Empire have watched gladiatorial games for fifteen centuries.<br><br>The box is currently empty except for imperial guards... and one figure moving with deadly intent.");
            emperorSeatBox.Exits.Add("south", 222);  // Back to upper gallery
            // No NPCs initially - assassination event spawns the assassin

            // Add Colosseum rooms
            rooms.Add(220, colosseumLowerGallery);
            rooms.Add(221, colosseumMidGallery);
            rooms.Add(222, colosseumUpperGallery);
            rooms.Add(223, emperorSeatBox);

            // ===== IMPERIAL HIGHWAY (Belum to Aevoria) =====
            // Journey progression: Belum North → Steppes → Shrublands → Quarry → Ruins → Marshlands → Aevoria

            // SEGMENT 1: Starting from west of Belum North Gate
            Room highway121 = CreateRoom(121, "imperialHighway121", "Imperial Highway - Western Approach",
                "The Imperial Highway stretches before you - a marvel of engineering paved with fitted stone that has endured for centuries. To your south, you can see the northern walls of Belum rising in the distance. The road is wide enough for four wagons to travel abreast, with drainage ditches on either side. Mile markers bearing the imperial eagle show the distance to major cities.");

            Room highway122 = CreateRoom(122, "imperialHighway122", "Imperial Highway - Belum Outskirts",
                "You're still within sight of Belum's north gate to the south, though the city is beginning to fade behind you. Small farms and homesteads dot the landscape here - this close to a major town, the roads are safe and settlement thrives. Smoke rises from chimneys, and you can hear the distant sound of cattle lowing in their pens.");

            // SEGMENT 2: Northern Steppes
            Room highway123 = CreateRoom(123, "imperialHighway123", "Imperial Highway - Edge of the Steppes",
                "The landscape begins to change as the road leads you northward. To the north, vast grasslands stretch to the horizon - the legendary Northern Steppes where tribal horsemen once ruled before the Empire pacified them. The wind carries the scent of wild grass and distant rain. Civilization feels far away here.");

            Room highway124 = CreateRoom(124, "imperialHighway124", "Imperial Highway - Steppe Overlook",
                "The highway runs along a ridge here, offering a commanding view of the steppes to the north. Endless waves of grass ripple in the wind like a green ocean. You can see herds of wild horses in the distance, running free across the plains. This is the frontier - beautiful, vast, and untamed.");

            Room highway125 = CreateRoom(125, "imperialHighway125", "Imperial Highway - Steppe Crossing",
                "The road cuts directly through the steppes here. The landscape is deceptively flat - you can see for miles in every direction, which is both reassuring and unsettling. Imperial watchtowers dot the horizon at regular intervals, remnants of the campaigns that brought these lands under control. The wind never stops.");

            // SEGMENT 3: Shrublands transition
            Room highway126 = CreateRoom(126, "imperialHighway126", "Imperial Highway - Shrubland Border",
                "The terrain shifts as you continue north. To the north, the open steppes give way to dense shrubland - thick bushes and small twisted trees that grow in stubborn clusters. The Imperial Highway remains immaculate, but the wild country presses close on either side. You notice game trails crossing the road - deer, perhaps, or something larger.");

            Room highway127 = CreateRoom(127, "imperialHighway127", "Imperial Highway - Through the Shrublands",
                "Dense shrubland flanks the highway to the north, creating a wall of thorny vegetation that would be impassable without the road. The bushes are thick with berries - some edible, some poisonous, if you know which is which. Bird calls echo through the thickets, and you catch glimpses of movement in the shadows.");

            Room highway128 = CreateRoom(128, "imperialHighway128", "Imperial Highway - Shrubland Heights",
                "The road climbs slightly here, and you can look down on the shrublands spreading to the north like a rumpled green carpet. In the distance to the south, you can still make out Belum's walls - a reassuring sight of civilization in this wild country. The highway continues its inexorable march northward toward the capital.");

            // SEGMENT 4: Quarry region (east)
            Room highway129 = CreateRoom(129, "imperialHighway129", "Imperial Highway - Quarry Road Junction",
                "A side road branches off the highway here, leading east toward what appears to be an active quarry. You can hear the distant sound of hammers on stone and see dust rising from the excavation site. Wagons loaded with cut stone occasionally join the main highway here, their wheels worn deep into the road from years of heavy use. The quarry must supply stone for imperial construction projects.");

            Room highway130 = CreateRoom(130, "imperialHighway130", "Imperial Highway - Past the Quarry",
                "The quarry is clearly visible to the east now - a massive scar in the hillside where workers have been extracting limestone and marble for generations. You can see tiny figures moving among the cut stone, hear the rhythmic striking of tools. This is where the Empire's grandeur is literally carved from the earth. The highway continues north, paved with the very stone mined from these hills.");

            Room highway131 = CreateRoom(131, "imperialHighway131", "Imperial Highway - Beyond the Quarry",
                "The sounds of the quarry fade behind you as the road continues northward. The landscape here is scarred by old excavation sites - places where the stone was exhausted decades or centuries ago and nature is slowly reclaiming the land. Wildflowers grow in the abandoned pits, and trees sprout from cracks in discarded stone. Progress marches on, leaving ruins in its wake.");

            // SEGMENT 5: Ancient Ruins (west)
            Room highway132 = CreateRoom(132, "imperialHighway132", "Imperial Highway - Ruins Approach",
                "To the west, massive stone structures rise from the landscape - ruins far older than the Empire itself. These are pre-imperial constructions, built by civilizations whose very names have been forgotten. The Imperial Highway runs alongside these ancient monuments, a newer road beside a far older mystery. Scholars sometimes camp here, studying the weathered stones.");

            Room highway133 = CreateRoom(133, "imperialHighway133", "Imperial Highway - Shadow of the Ancients",
                "The ruins loom to the west - cyclopean walls built from stones so massive it's hard to imagine how they were moved. No mortar holds them together; they're fitted with such precision that a knife blade couldn't slip between them. The Empire claims these lands, but these structures predate imperial history by millennia. What empire fell here? What catastrophe reduced them to ruins?");

            Room highway134 = CreateRoom(134, "imperialHighway134", "Imperial Highway - Ancient Gateway",
                "A massive archway stands to the west - part of the ruins, still standing after countless centuries. The Imperial Highway passes directly beside it, as if the road builders deliberately positioned the route to acknowledge these ancient works. Strange symbols are carved into the arch's keystone - not imperial script, not any language you recognize. Some mysteries endure.");

            Room highway135 = CreateRoom(135, "imperialHighway135", "Imperial Highway - Leaving the Ruins",
                "The ancient ruins gradually fade behind you to the west as the highway continues its northern march. You take one last look at those massive stones, those impossible walls. The Empire is powerful, yes - but it stands on the bones of older powers. The road ahead is well-maintained, paved by imperial engineers who perhaps never wondered what came before.");

            // SEGMENT 6: Marshlands (east)
            Room highway136 = CreateRoom(136, "imperialHighway136", "Imperial Highway - Marsh Border",
                "The air grows thick and humid as marshlands appear to the east. You can smell the wetlands - a mix of rotting vegetation, stagnant water, and rich mud. The Imperial Highway is raised here, built on a causeway to keep it above the flood plain. This must have been an incredible engineering feat, creating a solid road through swampland.");

            Room highway137 = CreateRoom(137, "imperialHighway137", "Imperial Highway - Through the Marshes",
                "Marshland stretches to the east - pools of dark water, stands of reeds, twisted trees draped with moss. The Imperial Highway cuts straight through it all, an arrow of civilization through the wild. You can hear frogs croaking, insects buzzing, and the occasional splash of something moving through the water. The road beneath your feet is solid, but the world on either side is uncertain, shifting.");

            Room highway138 = CreateRoom(138, "imperialHighway138", "Imperial Highway - Marsh Causeway",
                "The raised causeway continues through the marshlands to the east. Imperial engineering at its finest - drainage channels on both sides keep water from undermining the road, and stone abutments support the structure where the ground is softest. Birds wade in the shallow waters, hunting for fish. The marsh is beautiful in its own way, but you're grateful for the solid road.");

            // SEGMENT 7: Approaching Aevoria
            Room highway139 = CreateRoom(139, "imperialHighway139", "Imperial Highway - Northern Reaches",
                "The marshlands finally give way to firmer ground as you continue north. The landscape here is well-cultivated - farms and estates owned by wealthy nobles who enjoy proximity to the capital without living in the city itself. The highway is busier here; you pass other travelers, merchants, imperial messengers on fast horses. Civilization thickens with every mile northward.");

            Room highway140 = CreateRoom(140, "imperialHighway140", "Imperial Highway - Aevoria Approaches",
                "The traffic increases steadily as you travel north. Wagons laden with goods, nobles in carriages, pilgrims on foot - all making their way to or from the Eternal City. To the far north, you can just make out a change in the horizon - something massive and white that must be Aevoria itself. The capital, heart of the Empire, draws near.");

            Room highway141 = CreateRoom(141, "imperialHighway141", "Imperial Highway - Capital Road",
                "Aevoria is clearly visible now to the north - white marble buildings gleaming in the distance, the Colosseum's massive arches rising above the city walls. The highway here is immaculate, swept daily by work crews who maintain the approaches to the capital. Imperial banners flutter from posts along the road. You're in the heart of the Empire now.");

            // Final room connecting to Aevoria
            Room highway142 = CreateRoom(142, "imperialHighway142", "Imperial Highway - Aevoria Gates",
                "The Imperial Highway terminates at the massive western gates of Aevoria. The city walls rise fifty feet high, built from gleaming white marble that seems to glow in the sunlight. Guards in ceremonial armor stand at attention, checking travelers entering the capital. Beyond the gates, you can see the city itself - the Eternal City, fifteen centuries old, seat of imperial power. You've arrived.");

            // Add Imperial Highway rooms
            rooms.Add(121, highway121);
            rooms.Add(122, highway122);
            rooms.Add(123, highway123);
            rooms.Add(124, highway124);
            rooms.Add(125, highway125);
            rooms.Add(126, highway126);
            rooms.Add(127, highway127);
            rooms.Add(128, highway128);
            rooms.Add(129, highway129);
            rooms.Add(130, highway130);
            rooms.Add(131, highway131);
            rooms.Add(132, highway132);
            rooms.Add(133, highway133);
            rooms.Add(134, highway134);
            rooms.Add(135, highway135);
            rooms.Add(136, highway136);
            rooms.Add(137, highway137);
            rooms.Add(138, highway138);
            rooms.Add(139, highway139);
            rooms.Add(140, highway140);
            rooms.Add(141, highway141);
            rooms.Add(142, highway142);

            // ============================================
            // DUNGEON TEST AREA (Rooms 900-920)
            // ============================================

            // Room 900: Hub/Entrance
            Room dungeonHub = CreateRoom(900, "dungeonHub", "Ancient Ruins - Entrance",
                "You stand before a massive stone archway, worn smooth by countless centuries. The stonework predates the Empire by millennia - cyclopean blocks fitted together with impossible precision. Strange symbols are carved into the arch's surface, their meaning lost to time. Beyond the archway, stone steps descend into darkness. The air that rises from below is cool and carries the faint scent of ancient dust and something else... something watchful.");
            dungeonHub.Exits.Add("down", 901);  // To Floor 1

            // FLOOR 1: Linear Layout (901-905) - Levels 1-5
            Room floor1Room1 = CreateRoom(901, "dungeonF1R1", "Ancient Ruins - Entry Chamber",
                "The stone steps lead down into a vast chamber lit by an eerie phosphorescent moss growing on the walls. Broken pottery and ancient weapons litter the floor. Two satyrs prowl among the rubble, their goat-like legs clicking on the stone. They turn toward you, yellow eyes gleaming with feral hunger.");
            floor1Room1.Exits.Add("up", 900);   // Back to hub
            floor1Room1.Exits.Add("east", 902);
            floor1Room1.OriginalNPCs.Add(npcs["Satyr"].Clone());
            floor1Room1.OriginalNPCs.Add(npcs["Satyr"].Clone());
            floor1Room1.CanRespawn = false;  // Dungeon enemies don't respawn normally - only on full reset

            Room floor1Room2 = CreateRoom(902, "dungeonF1R2", "Ancient Ruins - Weathered Corridor",
                "A long corridor stretches eastward, its walls decorated with faded frescoes depicting long-forgotten battles. A harpy perches on a broken pillar, preening its bronze-feathered wings. At its feet, a satyr gnaws on old bones. Both creatures notice your presence.");
            floor1Room2.Exits.Add("west", 901);
            floor1Room2.Exits.Add("east", 903);
            floor1Room2.OriginalNPCs.Add(npcs["Harpy"].Clone());
            floor1Room2.OriginalNPCs.Add(npcs["Satyr"].Clone());
            floor1Room2.CanRespawn = false;

            Room floor1Room3 = CreateRoom(903, "dungeonF1R3", "Ancient Ruins - Grand Chamber",
                "The corridor opens into a grand chamber with a vaulted ceiling supported by massive stone columns. Two giant scorpions, each as large as a cart, scuttle across the floor. Their chitinous armor gleams in the dim light, and their stingers drip with venom.");
            floor1Room3.Exits.Add("west", 902);
            floor1Room3.Exits.Add("east", 904);
            floor1Room3.OriginalNPCs.Add(npcs["Giant Scorpion"].Clone());
            floor1Room3.OriginalNPCs.Add(npcs["Giant Scorpion"].Clone());
            floor1Room3.CanRespawn = false;

            Room floor1Room4 = CreateRoom(904, "dungeonF1R4", "Ancient Ruins - Bone-Strewn Passage",
                "This narrow passage is littered with ancient bones - human and otherwise. Three skeleton warriors stand in an eternal vigil, their empty eye sockets glowing with spectral blue light. As you enter, they raise their rusted weapons in unison.");
            floor1Room4.Exits.Add("west", 903);
            floor1Room4.Exits.Add("east", 905);
            floor1Room4.OriginalNPCs.Add(npcs["Skeleton Warrior"].Clone());
            floor1Room4.OriginalNPCs.Add(npcs["Skeleton Warrior"].Clone());
            floor1Room4.OriginalNPCs.Add(npcs["Skeleton Warrior"].Clone());
            floor1Room4.CanRespawn = false;

            Room floor1Room5 = CreateRoom(905, "dungeonF1R5", "Ancient Ruins - Bronze Treasury",
                "The passage opens into a circular chamber with a domed ceiling. Ancient bronze weapons and armor line the walls on racks and stands. A harpy roosts on a bronze chandelier above, while two satyrs guard a stone chest in the center of the room. Stone steps descend deeper into the ruins.");
            floor1Room5.Exits.Add("west", 904);
            // floor1Room5.Exits.Add("down", 906);  // Added dynamically on victory
            floor1Room5.OriginalNPCs.Add(npcs["Harpy"].Clone());
            floor1Room5.OriginalNPCs.Add(npcs["Satyr"].Clone());
            floor1Room5.OriginalNPCs.Add(npcs["Satyr"].Clone());
            floor1Room5.Items.Add("bronze gladius");
            floor1Room5.Items.Add("reinforced bow");
            floor1Room5.Items.Add("bronze staff");
            floor1Room5.Items.Add("bronze breastplate");
            floor1Room5.CanRespawn = false;

            // FLOOR 2: Branching Layout (906-910) - Levels 6-10
            Room floor2Room1 = CreateRoom(906, "dungeonF2R1", "Ancient Ruins - Enchanted Hall",
                "The stairs lead into a vast hall whose walls shimmer with residual magic. A bronze automaton stands in the center of the room, its eyes glowing with arcane light. Ancient gears whir and click as it turns to face you.");
            floor2Room1.Exits.Add("up", 905);  // Back to Floor 1
            floor2Room1.Exits.Add("east", 907);
            floor2Room1.OriginalNPCs.Add(npcs["Bronze Automaton"].Clone());
            floor2Room1.CanRespawn = false;

            Room floor2Room2 = CreateRoom(907, "dungeonF2R2", "Ancient Ruins - Central Crossroads",
                "This chamber serves as a nexus point, with passages leading in three directions. Two centaur scouts patrol the area, their hooves echoing on the stone floor. They draw their bows as you approach.");
            floor2Room2.Exits.Add("west", 906);
            floor2Room2.Exits.Add("north", 908);
            floor2Room2.Exits.Add("east", 909);
            floor2Room2.OriginalNPCs.Add(npcs["Centaur Scout"].Clone());
            floor2Room2.OriginalNPCs.Add(npcs["Centaur Scout"].Clone());
            floor2Room2.CanRespawn = false;

            Room floor2Room3 = CreateRoom(908, "dungeonF2R3", "Ancient Ruins - Northern Treasury",
                "A side chamber filled with piles of ancient gold coins and gemstones. A gorgon stands guard over the treasure, her serpentine hair writhing. You feel her petrifying gaze upon you.");
            floor2Room3.Exits.Add("south", 907);
            floor2Room3.OriginalNPCs.Add(npcs["Gorgon"].Clone());
            floor2Room3.Items.Add("potion");
            floor2Room3.Items.Add("potion");
            floor2Room3.CanRespawn = false;

            Room floor2Room4 = CreateRoom(909, "dungeonF2R4", "Ancient Ruins - Eastern Passage",
                "A long passage decorated with murals of ancient battles. Two furies hover in the air, their wings beating slowly. Flames flicker around their clawed hands.");
            floor2Room4.Exits.Add("west", 907);
            floor2Room4.Exits.Add("east", 910);
            floor2Room4.OriginalNPCs.Add(npcs["Fury"].Clone());
            floor2Room4.OriginalNPCs.Add(npcs["Fury"].Clone());
            floor2Room4.CanRespawn = false;

            Room floor2Room5 = CreateRoom(910, "dungeonF2R5", "Ancient Ruins - Enchanted Armory",
                "A magnificent chamber whose walls are lined with enchanted weapons and armor that pulse with magical energy. A bronze automaton and a gorgon guard a large stone chest. Stairs descend to the depths below.");
            floor2Room5.Exits.Add("west", 909);
            // floor2Room5.Exits.Add("down", 911);  // Added dynamically on victory
            floor2Room5.OriginalNPCs.Add(npcs["Bronze Automaton"].Clone());
            floor2Room5.OriginalNPCs.Add(npcs["Gorgon"].Clone());
            floor2Room5.Items.Add("enchanted spatha");
            floor2Room5.Items.Add("stormbow");
            floor2Room5.Items.Add("crystal staff");
            floor2Room5.Items.Add("blessed cuirass");
            floor2Room5.CanRespawn = false;

            // FLOOR 3: Linear Layout (911-915) - Levels 11-15
            Room floor3Room1 = CreateRoom(911, "dungeonF3R1", "Ancient Ruins - Minotaur's Domain",
                "The stairs open into a massive circular arena. The floor is stained with ancient blood. A minotaur paces in the center, its massive axe resting on one shoulder. It bellows a challenge as you enter.");
            floor3Room1.Exits.Add("up", 910);  // Back to Floor 2
            floor3Room1.Exits.Add("east", 912);
            floor3Room1.OriginalNPCs.Add(npcs["Minotaur"].Clone());
            floor3Room1.CanRespawn = false;

            Room floor3Room2 = CreateRoom(912, "dungeonF3R2", "Ancient Ruins - Serpent Corridor",
                "A long corridor whose walls are carved with images of serpents and dark rituals. Two lamias slither through the shadows, their hypnotic eyes fixed on you.");
            floor3Room2.Exits.Add("west", 911);
            floor3Room2.Exits.Add("east", 913);
            floor3Room2.OriginalNPCs.Add(npcs["Lamia"].Clone());
            floor3Room2.OriginalNPCs.Add(npcs["Lamia"].Clone());
            floor3Room2.CanRespawn = false;

            Room floor3Room3 = CreateRoom(913, "dungeonF3R3", "Ancient Ruins - Grand Throne Hall",
                "An enormous hall with a high vaulted ceiling. A single massive eye opens above you - a cyclops awakens from its slumber. At its feet, two skeleton warriors stand ready, their bones reinforced with dark magic.");
            floor3Room3.Exits.Add("west", 912);
            floor3Room3.Exits.Add("east", 914);
            floor3Room3.OriginalNPCs.Add(npcs["Cyclops"].Clone());
            floor3Room3.OriginalNPCs.Add(npcs["Skeleton Warrior"].Clone());
            floor3Room3.OriginalNPCs.Add(npcs["Skeleton Warrior"].Clone());
            floor3Room3.CanRespawn = false;

            Room floor3Room4 = CreateRoom(914, "dungeonF3R4", "Ancient Ruins - Petrified Antechamber",
                "This chamber is filled with dozens of stone statues - victims of the medusa who dwells here. The serpent-haired gorgon queen stands beside a massive minotaur. Both guardians turn toward you.");
            floor3Room4.Exits.Add("west", 913);
            floor3Room4.Exits.Add("east", 915);
            floor3Room4.OriginalNPCs.Add(npcs["Medusa"].Clone());
            floor3Room4.OriginalNPCs.Add(npcs["Minotaur"].Clone());
            floor3Room4.CanRespawn = false;

            Room floor3Room5 = CreateRoom(915, "dungeonF3R5", "Ancient Ruins - Legendary Vault",
                "A treasure vault of legendary proportions. Weapons and armor of incredible craftsmanship line the walls. A cyclops and a medusa guard the central chest. Steps lead down to the final depths of the ruins.");
            floor3Room5.Exits.Add("west", 914);
            // floor3Room5.Exits.Add("down", 916);  // Added dynamically on victory
            floor3Room5.OriginalNPCs.Add(npcs["Cyclops"].Clone());
            floor3Room5.OriginalNPCs.Add(npcs["Medusa"].Clone());
            floor3Room5.Items.Add("hero's blade");
            floor3Room5.Items.Add("gorgon's bane");
            floor3Room5.Items.Add("medusa's wand");
            floor3Room5.Items.Add("griffon hide armor");
            floor3Room5.CanRespawn = false;

            // FLOOR 4: Branching Layout (916-920) - Levels 16-20
            Room floor4Room1 = CreateRoom(916, "dungeonF4R1", "Ancient Ruins - Gates of the Divine",
                "The deepest level of the ruins opens into a cathedral-like space. Divine light filters down from cracks in the ceiling far above. A chimera and a three-headed cerberus guard the way forward. This is the realm of legends.");
            floor4Room1.Exits.Add("up", 915);  // Back to Floor 3
            floor4Room1.Exits.Add("east", 917);
            floor4Room1.OriginalNPCs.Add(npcs["Chimera"].Clone());
            floor4Room1.OriginalNPCs.Add(npcs["Cerberus"].Clone());
            floor4Room1.CanRespawn = false;

            Room floor4Room2 = CreateRoom(917, "dungeonF4R2", "Ancient Ruins - Hydra's Lair",
                "A vast chamber with a pool of dark water in the center. A massive hydra rises from the depths, its multiple heads swaying hypnotically. Each head breathes a different elemental attack.");
            floor4Room2.Exits.Add("west", 916);
            floor4Room2.Exits.Add("north", 918);
            floor4Room2.Exits.Add("east", 919);
            floor4Room2.OriginalNPCs.Add(npcs["Hydra"].Clone());
            floor4Room2.CanRespawn = false;

            Room floor4Room3 = CreateRoom(918, "dungeonF4R3", "Ancient Ruins - Divine Treasury",
                "A sanctum filled with divine artifacts and treasures beyond mortal comprehension. Two chimeras stand eternal watch over the hoard. The very air thrums with divine power.");
            floor4Room3.Exits.Add("south", 917);
            floor4Room3.OriginalNPCs.Add(npcs["Chimera"].Clone());
            floor4Room3.OriginalNPCs.Add(npcs["Chimera"].Clone());
            floor4Room3.Items.Add("potion");
            floor4Room3.Items.Add("potion");
            floor4Room3.Items.Add("restoration scroll");
            floor4Room3.CanRespawn = false;

            Room floor4Room4 = CreateRoom(919, "dungeonF4R4", "Ancient Ruins - Titan's Approach",
                "The passage opens to reveal a titanic statue that slowly begins to move. This is no statue - it's a titan, ancient guardian of this place. Its stone skin cracks as it takes its first step in centuries.");
            floor4Room4.Exits.Add("west", 917);
            floor4Room4.Exits.Add("east", 920);
            floor4Room4.OriginalNPCs.Add(npcs["Titan"].Clone());
            floor4Room4.CanRespawn = false;

            Room floor4Room5 = CreateRoom(920, "dungeonF4R5", "Ancient Ruins - Chamber of the Gods",
                "The heart of the ancient ruins - a chamber that predates the Empire by thousands of years. A titan and a hydra stand as final guardians before an altar laden with divine weapons and armor. Completing this challenge proves you worthy of legend. A shimmering portal stands ready to return you to the surface.");
            floor4Room5.Exits.Add("west", 919);
            floor4Room5.Exits.Add("up", 900);  // Portal back to hub
            floor4Room5.OriginalNPCs.Add(npcs["Titan"].Clone());
            floor4Room5.OriginalNPCs.Add(npcs["Hydra"].Clone());
            floor4Room5.Items.Add("titan's maul");
            floor4Room5.Items.Add("olympian greatbow");
            floor4Room5.Items.Add("divine scepter");
            floor4Room5.Items.Add("aegis of the gods");
            floor4Room5.CanRespawn = false;

            // Copy OriginalNPCs to NPCs for initial dungeon population
            foreach (var npc in floor1Room1.OriginalNPCs) floor1Room1.NPCs.Add(npc.Clone());
            foreach (var npc in floor1Room2.OriginalNPCs) floor1Room2.NPCs.Add(npc.Clone());
            foreach (var npc in floor1Room3.OriginalNPCs) floor1Room3.NPCs.Add(npc.Clone());
            foreach (var npc in floor1Room4.OriginalNPCs) floor1Room4.NPCs.Add(npc.Clone());
            foreach (var npc in floor1Room5.OriginalNPCs) floor1Room5.NPCs.Add(npc.Clone());
            foreach (var npc in floor2Room1.OriginalNPCs) floor2Room1.NPCs.Add(npc.Clone());
            foreach (var npc in floor2Room2.OriginalNPCs) floor2Room2.NPCs.Add(npc.Clone());
            foreach (var npc in floor2Room3.OriginalNPCs) floor2Room3.NPCs.Add(npc.Clone());
            foreach (var npc in floor2Room4.OriginalNPCs) floor2Room4.NPCs.Add(npc.Clone());
            foreach (var npc in floor2Room5.OriginalNPCs) floor2Room5.NPCs.Add(npc.Clone());
            foreach (var npc in floor3Room1.OriginalNPCs) floor3Room1.NPCs.Add(npc.Clone());
            foreach (var npc in floor3Room2.OriginalNPCs) floor3Room2.NPCs.Add(npc.Clone());
            foreach (var npc in floor3Room3.OriginalNPCs) floor3Room3.NPCs.Add(npc.Clone());
            foreach (var npc in floor3Room4.OriginalNPCs) floor3Room4.NPCs.Add(npc.Clone());
            foreach (var npc in floor3Room5.OriginalNPCs) floor3Room5.NPCs.Add(npc.Clone());
            foreach (var npc in floor4Room1.OriginalNPCs) floor4Room1.NPCs.Add(npc.Clone());
            foreach (var npc in floor4Room2.OriginalNPCs) floor4Room2.NPCs.Add(npc.Clone());
            foreach (var npc in floor4Room3.OriginalNPCs) floor4Room3.NPCs.Add(npc.Clone());
            foreach (var npc in floor4Room4.OriginalNPCs) floor4Room4.NPCs.Add(npc.Clone());
            foreach (var npc in floor4Room5.OriginalNPCs) floor4Room5.NPCs.Add(npc.Clone());

            // Add all dungeon rooms to dictionary
            rooms.Add(900, dungeonHub);
            rooms.Add(901, floor1Room1);
            rooms.Add(902, floor1Room2);
            rooms.Add(903, floor1Room3);
            rooms.Add(904, floor1Room4);
            rooms.Add(905, floor1Room5);
            rooms.Add(906, floor2Room1);
            rooms.Add(907, floor2Room2);
            rooms.Add(908, floor2Room3);
            rooms.Add(909, floor2Room4);
            rooms.Add(910, floor2Room5);
            rooms.Add(911, floor3Room1);
            rooms.Add(912, floor3Room2);
            rooms.Add(913, floor3Room3);
            rooms.Add(914, floor3Room4);
            rooms.Add(915, floor3Room5);
            rooms.Add(916, floor4Room1);
            rooms.Add(917, floor4Room2);
            rooms.Add(918, floor4Room3);
            rooms.Add(919, floor4Room4);
            rooms.Add(920, floor4Room5);

            return rooms;
        }

        static Room CreateRoom(int numericId, string id, string title, string description, Dictionary<string, int> exits = null, List<string> items = null)
        {
            Room room = new Room();
            room.NumericId = numericId;
            room.Id = id;
            room.Title = title;
            room.Description = description;

            if (exits != null)
                room.Exits = exits;
            if (items != null)
                room.Items = items;

            return room;
        }
    }
}