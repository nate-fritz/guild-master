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
            Room guildPath = CreateRoom(6, "guildPath", "A Dirt Path", "You're on a wide dirt path that runs roughly north to south. Rolling grass covered hills flank either side of the path, with the occasional tree breaking up the sea of green.  To the south you can still see the guild hall.  The wilderness stretches for as far as you can see to the north.");
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
            Room deepCave = CreateRoom(18, "deepCave", "Bandit Cave - Deep Cavern", "You've descended deep into the mountain. The air here is cold and still. Strange rock formations create eerie shapes in the torchlight. Water drips steadily somewhere in the darkness. The cave continues east toward the sound of loud voices and running water.  The way north leads back toward the cave exit.");
            deepCave.Exits.Add("north", 17);
            deepCave.Exits.Add("east", 21);
            deepCave.NPCs.Add(npcs["Bandit Enforcer"].Clone());
            deepCave.NPCs.Add(npcs["Bandit Enforcer"].Clone());

            // Updated
            Room floodedChamber = CreateRoom(19, "floodedChamber", "Bandit Cave - Flooded Chamber", "An underground stream flows through this chamber, the water black and swift. A narrow ledge runs along the eastern wall. Someone is standing guard here - a woman with a bow. She doesn't look like the other bandits. The only way forward is west, across a rickety wooden bridge.");
            floodedChamber.Exits.Add("north", 14);
            floodedChamber.Exits.Add("south", 20);
            floodedChamber.NPCs.Add(npcs["Livia"].Clone());  // Recruitable Venator

            // Updated
            Room undergroundRiver = CreateRoom(20, "undergroundRiver", "Bandit Cave - Underground River", "A narrow chasm opens up in the middle of this cavern, with a rickety looking wood and rope bridge spanning across it.  It's too dark to see the bottom of the chasm, but you can faintly hear the flowing water of an underground river far below you.  You hear loud voices to the west, and to the distant north is the cave's entrance.");
            undergroundRiver.Exits.Add("north", 19);
            undergroundRiver.Exits.Add("west", 21);

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

            Room pathWithCart = CreateRoom(42, "pathWithCart", "A wooded path with a broken cart", "As you enter this section of the wooded path, you notice a large wooden cart in a ditch on the side of the road.  The wheels are broken and the old wood is mossy and cracked.  It looks as though it's been here for ages.");
            pathWithCart.Exits.Add("west", 41);
            pathWithCart.Exits.Add("east", 43);
            pathWithCart.Items.Add("cart");

            Room nearFallenTree = CreateRoom(43, "nearFallenTree", "A wooded path near a fallen tree", "A gigantic fallen tree blocks the path here. You would have some difficulty climbing over it, but you notice a break in the trunk just south of the path that you can squeeze through to continue along.  As you approach the gap, you notice a hooded man crouching down, searching through the dirt.");
            nearFallenTree.Exits.Add("west", 42);
            nearFallenTree.Exits.Add("east", 44);
            nearFallenTree.NPCs.Add(npcs["Silvacis"]);  // Silvacis is unique, no clone needed

            Room deepForest = CreateRoom(44, "deepForest", "A deep forest clearing", "This secluded clearing is barely visible from the path. A small campfire smolders near a worn bedroll, and weapons are carefully arranged on a fallen log. Someone has been living here.  You can hear a stream to the south.");
            deepForest.Exits.Add("west", 43);
            deepForest.Exits.Add("south", 45);
            deepForest.NPCs.Add(npcs["Braxus"]);  // Braxus is unique, no clone needed

            Room forestStream = CreateRoom(45, "forestStream", "A stream running through the forest", "You stand before a shallow bubbling stream that runs east to west through the forest.  A waterlogged stump is stubbornly rooted in the center of the stream, the current splitting around it.  You can smell a campfire to the north. To the south a barely worn path continues into the forest, which gets darker the further you go.");
            forestStream.Exits.Add("north", 44);
            forestStream.Exits.Add("south", 46);
            forestStream.Items.Add("stump");

            Room forestBend = CreateRoom(46, "forestBend", "A bend in the forest path", "The path through the forest curves from the north to the east.  The path is looking less worn here, and the canopy overhead is so thick that only a few thin beams of light shine through.  You hear rustling leaves all around you, as though the creatures of this forest are constantly on the move.  To the north you can hear a stream, and to the east the path continues into the deeper parts of the woods.");
            forestBend.Exits.Add("north", 45);
            forestBend.Exits.Add("east", 47);
            forestBend.NPCs.Add(npcs["Dire Wolf"].Clone());  // Clone dire wolf
            forestBend.NPCs.Add(npcs["Bandit"].Clone());  // Clone bandit  
            forestBend.NPCs.Add(npcs["Bandit Thug"].Clone());  // Clone bandit thug
            forestBend.CanRespawn = true;
            forestBend.RespawnTimeHours = 16f;
            forestBend.OriginalNPCs.Add(npcs["Dire Wolf"].Clone());
            forestBend.OriginalNPCs.Add(npcs["Bandit"].Clone());
            forestBend.OriginalNPCs.Add(npcs["Bandit Thug"].Clone());

            Room abandonedCampsite = CreateRoom(47, "abandonedCampsite", "An abandoned campsite", "You come across the charred remains of a long abandoned campsite.  You notice a charred wooden chest sticking out of one of the several ash piles.  The west leads out of the forest, while to the east the forest grows wildly.");
            abandonedCampsite.Exits.Add("west", 46);
            abandonedCampsite.Exits.Add("east", 48);
            abandonedCampsite.Items.Add("chest");

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

            Room foggyPath = CreateRoom(53, "foggyPath", "A foggy path", "A low fog clings to the ground here, obscuring your feet. The mist seems to swirl and shift of its own accord. You can barely make out paths to the north and east.");
            foggyPath.Exits.Add("south", 54);

            Room wildernessTrail = CreateRoom(54, "wildernessTrail", "A wilderness trail", "A narrow trail winds through dense underbrush. Animal tracks crisscross the path - some disturbingly large.");
            wildernessTrail.Exits.Add("north", 55);

            Room fungalGrove = CreateRoom(55, "fungalGrove", "A fungal grove", "Enormous mushrooms grow in clusters here, some taller than a man. Their caps glow faintly with an eerie bioluminescence. The air smells earthy and strange.");
            fungalGrove.Exits.Add("west", 56);
            fungalGrove.Exits.Add("east", 54);

            Room mossyClearingSouth = CreateRoom(56, "mossyClearingSouth", "A mossy clearing", "Another mossy clearing, similar to the one you may have seen to the north. Are you going in circles? The forest all looks the same here.");
            mossyClearingSouth.Exits.Add("north", 55);
            mossyClearingSouth.Exits.Add("south", 57);

            Room twistingPath = CreateRoom(57, "twistingPath", "A twisting path", "The path twists and turns here, making it difficult to maintain your bearings. You're not entirely sure which direction you came from.");
            twistingPath.Exits.Add("north", 56);
            twistingPath.Exits.Add("west", 58);

            // Needs update
            Room forestEdge = CreateRoom(58, "forestEdge", "The forest's edge", "The trees thin here and you can see open sky to the east. The forest seems reluctant to let you go, branches reaching out like grasping fingers. But the exit is clear.");
            forestEdge.Exits.Add("east", 57);
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
            belumApproach.NPCs.Add(npcs["Marcus"].Clone());  // sentry quest giver
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

            // Needs update
            Room townHall = CreateRoom(87, "townHall", "Belum - Town Hall", "The administrative center of Belum. An imposing building with marble columns and bronze doors. Guards stand at attention. A notice board displays official proclamations.");
            townHall.Exits.Add("north", 88);
            townHall.Exits.Add("south", 86);
            townHall.Exits.Add("east", 84);
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
            rooms.Add(42, pathWithCart);
            rooms.Add(43, nearFallenTree);
            rooms.Add(44, deepForest);
            rooms.Add(45, forestStream);
            rooms.Add(46, forestBend);
            rooms.Add(47, abandonedCampsite);
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