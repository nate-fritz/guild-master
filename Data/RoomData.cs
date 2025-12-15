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

            Room bedroom = CreateRoom(1, "guildHallBedroom", "The Guild Hall - Bedroom", "You're in a small bedroom with a bed and a nightstand.  There's a small locked chest at the foot of the bed.  There's a desk with a small copper mug sitting on it. To the east is a door that leads to a hallway.");
            bedroom.Exits.Add("east", 2);
            bedroom.Items.Add("mug");
            bedroom.Items.Add("note");
            bedroom.Items.Add("chest");

            Room hallway = CreateRoom(2, "guildHallHallway", "The Guild Hall - Hallway", "You're in a narrow hallway that runs north to south.  To the west is your bedroom.  A study is to the south, and a large common area is open to the north.");
            hallway.Exits.Add("west", 1);
            hallway.Exits.Add("north", 4);
            hallway.Exits.Add("south", 3);

            Room study = CreateRoom(3, "guildHallStudy", "The Guild Hall - Study", "You're in a dusty study filled with old books, scrolls, and other trinkets.  The hallway that you entered from is to the north.");
            study.Exits.Add("north", 2);
            study.Items.Add("desk");
            study.Items.Add("energy potion");
            study.Items.Add("restoration scroll");
            study.Items.Add("leather armor");
            study.Items.Add("iron gladius");

            Room commonArea = CreateRoom(4, "guildHallCommonArea", "The Guild Hall - Common Area", "A large common area with tables, chairs, and couches in front of a large fireplace.  You imagine this room was once quite lively, but today it sits unused.  The main door to the guildhall is to the north, and a hallway is to the south.");
            commonArea.Exits.Add("south", 2);
            commonArea.Exits.Add("north", 5);

            Room frontDoor = CreateRoom(5, "guildHallFrontDoor", "The Guild Hall - Front Door", "You stand before a modest two-story building made of grey stone with a yellow thatched roof. Ivy climbs the walls, and the roof looks a bit worse for wear, but considering it's been largely abandoned in recent years, it's relatively well maintained. Two large wooden doors stand at the back of the portico at the front of the guildhall. A hanging sign sways lazily in the wind, its painted lettering worn away to the point of being indecipherable.");
            frontDoor.Exits.Add("south", 4);
            frontDoor.Exits.Add("north", 6);

            Room guildPath = CreateRoom(6, "guildPath", "A Dirt Path", "You're on a wide dirt path that runs roughly north to south. Rolling grass covered hills flank either side of the path, with the occasional tree breaking up the sea of green.  To the south you can still see the guild hall.  The wilderness stretches for as far as you can see to the north.");
            guildPath.Exits.Add("south", 5);
            guildPath.Exits.Add("north", 7);

            Room theCrossRoads = CreateRoom(7, "theCrossRoads", "A crossroads", "You enter a crossroads that extends in the four cardinal directions.  A wooden sign post has four signs pointing towards each. \n\nNorth:   The Town of Belum\nEast:    Hircinian Forest\nSouth:   [this sign looks long neglected, and can't be read]\nWest:    Avinten Foothills");
            theCrossRoads.Exits.Add("south", 6);
            theCrossRoads.Exits.Add("west", 8);
            theCrossRoads.Exits.Add("east", 40);
            theCrossRoads.Exits.Add("north", 68);
            theCrossRoads.Items.Add("neglected sign");
            theCrossRoads.NPCs.Add(npcs["Gaius"]);  // Gaius is unique, no clone needed

            Room westPath = CreateRoom(8, "westPath", "A path west of the crossroads", "You are west of The Crossroads on a wide dirt path.  In some areas, where the road is deeply pitted from use and weather, one might notice ancient cobblestone.  Further west the skyline is dominated by distant snow tipped mountains.");
            westPath.Exits.Add("east", 7);
            westPath.Exits.Add("west", 9);

            Room westernBend = CreateRoom(9, "westernBend", "A bend in the road", "The road bends sharply before resuming a west-to-east orientation.  To the west you see a nearby mountain range.  To the far east you can barely make out a crossroads.");
            westernBend.Exits.Add("east", 8);
            westernBend.Exits.Add("west", 10);
            westernBend.NPCs.Add(npcs["Bandit"].Clone());
            westernBend.CanRespawn = true;
            westernBend.RespawnTimeHours = 16f;
           
            westernBend.OriginalNPCs.Add(npcs["Bandit"].Clone());

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

            Room gaiusFarmHouse = CreateRoom(11, "gaiusFarmHouse", "Gaius' Farmhouse", "A large stone house with baked clay roof tiles stands before you, surrounded by the fields and other buildings that make up Gaius' farm.");
            gaiusFarmHouse.Exits.Add("east", 10);
            gaiusFarmHouse.Exits.Add("west", 12);
            gaiusFarmHouse.NPCs.Add(npcs["Bandit Leader"]);
            gaiusFarmHouse.CanRespawn = true;
            gaiusFarmHouse.RespawnTimeHours = 16f;
            gaiusFarmHouse.OriginalNPCs.Add(npcs["Bandit Leader"].Clone());

            Room lowerSlopes = CreateRoom(12, "lowerSlopes", "The Lower Slopes", "Here the rolling fields of the foothills meet the base of the mountain, which begins to slope sharply upward to the west.  The mountain's snow-touched peaks are nearly hidden by wispy clouds above.  The steep incline and high winds make a trek up the mountain seem ill advised.  To the south you see a cave mouth, and to the east is Gaius' farm.");
            lowerSlopes.Exits.Add("east", 11);
            lowerSlopes.Exits.Add("west", 13);

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
            forestBend.Exits.Add("south", 53);
            forestBend.NPCs.Add(npcs["Dire Wolf"].Clone());  // Clone dire wolf
            forestBend.NPCs.Add(npcs["Bandit"].Clone());  // Clone bandit  
            forestBend.NPCs.Add(npcs["Bandit Thug"].Clone());  // Clone bandit thug
            forestBend.CanRespawn = true;
            forestBend.RespawnTimeHours = 16f;
            forestBend.OriginalNPCs.Add(npcs["Dire Wolf"].Clone());
            forestBend.OriginalNPCs.Add(npcs["Bandit"].Clone());
            forestBend.OriginalNPCs.Add(npcs["Bandit Thug"].Clone());

            // NEW MOUNTAIN PATH ROOMS

            // ALL OF THIS NEEDS UPDATED UNLESS MARKED OTHERWISE
            Room mountainPath = CreateRoom(13, "mountainPath", "Mountain Path", "A narrow path winds up the mountainside. The air grows thinner and colder as you climb. Loose rocks make footing treacherous. To the east you can see Gaius' farm far below. The path continues west, climbing higher.");
            mountainPath.Exits.Add("east", 12);
            mountainPath.Exits.Add("west", 14);
            mountainPath.NPCs.Add(npcs["Mountain Bandit"].Clone());
            mountainPath.NPCs.Add(npcs["Mountain Bandit"].Clone());
            mountainPath.NPCs.Add(npcs["Mountain Bandit"].Clone());
            mountainPath.CanRespawn = true;
            mountainPath.RespawnTimeHours = 12f;
            mountainPath.OriginalNPCs.Add(npcs["Mountain Bandit"].Clone());
            mountainPath.OriginalNPCs.Add(npcs["Mountain Bandit"].Clone());
            mountainPath.OriginalNPCs.Add(npcs["Mountain Bandit"].Clone());

            Room rockyOutcrop = CreateRoom(14, "rockyOutcrop", "Rocky Outcrop", "The path opens onto a wide rocky ledge. Ice and snow cover the ground here, and the temperature has dropped noticeably. The howling wind carries the sound of distant wolves. To the east the path descends, while to the west a dark cave entrance beckons.");
            rockyOutcrop.Exits.Add("east", 13);
            rockyOutcrop.Exits.Add("west", 15);
            rockyOutcrop.NPCs.Add(npcs["Frost Wolf"].Clone());
            rockyOutcrop.NPCs.Add(npcs["Frost Wolf"].Clone());
            rockyOutcrop.NPCs.Add(npcs["Frost Wolf"].Clone());
            rockyOutcrop.CanRespawn = true;
            rockyOutcrop.RespawnTimeHours = 12f;
            rockyOutcrop.OriginalNPCs.Add(npcs["Frost Wolf"].Clone());
            rockyOutcrop.OriginalNPCs.Add(npcs["Frost Wolf"].Clone());
            rockyOutcrop.OriginalNPCs.Add(npcs["Frost Wolf"].Clone());

            Room iceCavern = CreateRoom(15, "iceCavern", "Ice Cavern", "You enter a cavern entirely encased in ice. Frozen waterfalls create pillars of crystalline beauty. Strange shapes move within the ice walls - elementals of pure winter. The cave continues deeper to the west, while the entrance lies to the east.");
            iceCavern.Exits.Add("east", 14);
            iceCavern.Exits.Add("west", 16);
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

            Room mountainPeak = CreateRoom(16, "mountainPeak", "Mountain Peak", "You emerge from the cave onto the mountain peak. Clouds swirl around you at this altitude, and lightning crackles in the distance. Massive eagles circle overhead, their cries echoing across the peaks. To the east is the ice cavern. To the west, ancient stone steps lead to what appears to be ruins.");
            mountainPeak.Exits.Add("east", 15);
            mountainPeak.Exits.Add("west", 17);
            mountainPeak.NPCs.Add(npcs["Thunder Eagle"].Clone());
            mountainPeak.NPCs.Add(npcs["Thunder Eagle"].Clone());
            mountainPeak.NPCs.Add(npcs["Thunder Eagle"].Clone());
            mountainPeak.CanRespawn = true;
            mountainPeak.RespawnTimeHours = 12f;
            mountainPeak.OriginalNPCs.Add(npcs["Thunder Eagle"].Clone());
            mountainPeak.OriginalNPCs.Add(npcs["Thunder Eagle"].Clone());
            mountainPeak.OriginalNPCs.Add(npcs["Thunder Eagle"].Clone());

            Room ancientAltar = CreateRoom(17, "ancientAltar", "Ancient Altar", "At the summit stands an ancient altar, carved from a single piece of black stone. Runes glow with an eerie light. A massive warrior in ancient armor stands before the altar, flanked by two elite guards. They turn to face you as you approach. The only exit is back to the east.");
            ancientAltar.Exits.Add("east", 16);
            ancientAltar.NPCs.Add(npcs["Mountain Warlord"].Clone());
            ancientAltar.NPCs.Add(npcs["Elite Guard"].Clone());
            ancientAltar.NPCs.Add(npcs["Elite Guard"].Clone());
            ancientAltar.CanRespawn = true;
            ancientAltar.RespawnTimeHours = 24f; // Boss takes longer to respawn
            ancientAltar.OriginalNPCs.Add(npcs["Mountain Warlord"].Clone());
            ancientAltar.OriginalNPCs.Add(npcs["Elite Guard"].Clone());
            ancientAltar.OriginalNPCs.Add(npcs["Elite Guard"].Clone());

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
            Room darkHollow = CreateRoom(48, "darkHollow", "A dark hollow", "The forest floor dips into a shadowy hollow here. Thick roots twist across the ground, and the air feels damp and still. Paths lead west and south, though both look equally uninviting.");
            darkHollow.Exits.Add("west", 47);
            darkHollow.Exits.Add("south", 54);

            Room tangledThicket = CreateRoom(49, "tangledThicket", "A tangled thicket", "Dense briars and thorny bushes crowd the path here. You can see faint trails leading in multiple directions, though none look well-traveled.");
            tangledThicket.Exits.Add("west", 48);
            tangledThicket.Exits.Add("south", 55);

            Room mossyClearingNorth = CreateRoom(50, "mossyClearingNorth", "A mossy clearing", "Soft green moss carpets this small clearing. Shafts of light filter through gaps in the canopy above. The forest feels slightly less oppressive here.");
            mossyClearingNorth.Exits.Add("west", 49);
            mossyClearingNorth.Exits.Add("south", 56);

            Room ancientOak = CreateRoom(51, "ancientOak", "An ancient oak", "A massive oak tree dominates this area, its trunk wider than three men standing together. Strange symbols are carved into its bark, worn smooth by time.");
            ancientOak.Exits.Add("west", 50);
            ancientOak.Exits.Add("south", 57);

            Room overgrownRuins = CreateRoom(52, "overgrownRuins", "Overgrown ruins", "Crumbling stone walls peek through the undergrowth here - the remains of some long-forgotten structure. Vines have reclaimed most of it.");
            overgrownRuins.Exits.Add("west", 51);
            overgrownRuins.Exits.Add("south", 58);

            // Middle row of forest (connects north and south)
            Room foggyPath = CreateRoom(53, "foggyPath", "A foggy path", "A low fog clings to the ground here, obscuring your feet. The mist seems to swirl and shift of its own accord. You can barely make out paths to the north and east.");
            foggyPath.Exits.Add("north", 46);  // Connects back to forestBend!
            foggyPath.Exits.Add("east", 54);

            Room wildernessTrail = CreateRoom(54, "wildernessTrail", "A wilderness trail", "A narrow trail winds through dense underbrush. Animal tracks crisscross the path - some disturbingly large.");
            wildernessTrail.Exits.Add("north", 48);
            wildernessTrail.Exits.Add("west", 53);
            wildernessTrail.Exits.Add("east", 55);
            wildernessTrail.Exits.Add("south", 59);

            Room fungalGrove = CreateRoom(55, "fungalGrove", "A fungal grove", "Enormous mushrooms grow in clusters here, some taller than a man. Their caps glow faintly with an eerie bioluminescence. The air smells earthy and strange.");
            fungalGrove.Exits.Add("north", 49);
            fungalGrove.Exits.Add("west", 54);
            fungalGrove.Exits.Add("east", 56);

            Room mossyClearingSouth = CreateRoom(56, "mossyClearingSouth", "A mossy clearing", "Another mossy clearing, similar to the one you may have seen to the north. Are you going in circles? The forest all looks the same here.");
            mossyClearingSouth.Exits.Add("north", 50);
            mossyClearingSouth.Exits.Add("west", 55);
            mossyClearingSouth.Exits.Add("east", 57);
            mossyClearingSouth.Exits.Add("south", 60);



            Room twistingPath = CreateRoom(57, "twistingPath", "A twisting path", "The path twists and turns here, making it difficult to maintain your bearings. You're not entirely sure which direction you came from.");
            twistingPath.Exits.Add("north", 51);
            twistingPath.Exits.Add("west", 56);
            twistingPath.Exits.Add("east", 58);


            // Needs update
            Room forestEdge = CreateRoom(58, "forestEdge", "The forest's edge", "The trees thin here and you can see open sky to the east. The forest seems reluctant to let you go, branches reaching out like grasping fingers. But the exit is clear.");
            forestEdge.Exits.Add("north", 52);
            forestEdge.Exits.Add("west", 57);
            forestEdge.Exits.Add("south", 61);

            // Southern row of forest
            // Needs update
            Room rottenLog = CreateRoom(59, "rottenLog", "A rotten log crossing", "A massive fallen log bridges a muddy depression here. The wood is soft and rotten - crossing it requires careful footing.");
            rottenLog.Exits.Add("north", 54);
            rottenLog.Exits.Add("east", 60);
            // Needs update
            Room wolfDen = CreateRoom(60, "wolfDen", "Near a wolf den", "You spot the dark entrance of a den dug into a hillside. Bones are scattered around the entrance. Something lives here - something hungry.");
            wolfDen.Exits.Add("north", 56);
            wolfDen.Exits.Add("west", 59);
            wolfDen.Exits.Add("east", 61);
            wolfDen.NPCs.Add(npcs["Dire Wolf"].Clone());
            wolfDen.NPCs.Add(npcs["Dire Wolf"].Clone());
            wolfDen.CanRespawn = true;
            wolfDen.RespawnTimeHours = 12f;
            wolfDen.OriginalNPCs.Add(npcs["Dire Wolf"].Clone());
            wolfDen.OriginalNPCs.Add(npcs["Dire Wolf"].Clone());


            // Needs update
            Room forestExit = CreateRoom(61, "forestExit", "Eastern forest exit", "The trees finally give way to open grassland. The dark forest looms behind you to the west. A worn path leads south toward... somewhere. Fresh air has never felt so good.");
            forestExit.Exits.Add("north", 58);
            forestExit.Exits.Add("west", 60);
            // Could add south exit to connect to something later

            // Add 47's east exit to connect to new rooms
            // UPDATE abandonedCampsite: abandonedCampsite.Exits.Add("east", 48);
            // UPDATE forestBend (46): forestBend.Exits.Add("south", 53);


            // ============================================
            // NORTH ROAD TO BELUM (Rooms 68-69)
            // ============================================

            // Needs update
            Room northRoad = CreateRoom(68, "northRoad", "The North Road", "The road north from the crossroads is well-maintained and shows signs of regular traffic. Wagon ruts line either side, and you can see the walls of a town in the distance. The crossroads lies to the south.");
            northRoad.Exits.Add("south", 7);
            northRoad.Exits.Add("north", 69);

            // Needs update
            Room belumApproach = CreateRoom(69, "belumApproach", "Approach to Belum", "The town walls of Belum rise before you, built of weathered grey stone. Guards patrol the battlements above. The main gate stands closed. A signpost reads: 'GATE CLOSED'.");
            belumApproach.Exits.Add("south", 68);
            // belumApproach.Exits.Add("north", 70);

            // BELUM - THE TOWN (Rooms 70-89)

            // Needs update
            Room belumSouthGate = CreateRoom(70, "belumSouthGate", "Belum - South Gate", "You pass through the southern gate of Belum. Guards in bronze armor eye travelers but make no move to stop you. The cobblestone streets are busy with merchants, locals, and fellow travelers. The main road continues north into the town center.");
            belumSouthGate.Exits.Add("south", 69);
            belumSouthGate.Exits.Add("north", 79);
            belumSouthGate.Exits.Add("east", 71);
            belumSouthGate.Exits.Add("west", 83);
            belumSouthGate.NPCs.Add(npcs["Town Guard"].Clone());

            // Needs update
            Room southMarket = CreateRoom(71, "southMarket", "Belum - South Market", "Stalls and carts line this section of the street, merchants hawking their wares. The smell of spices, leather, and fresh bread fills the air. The noise of commerce is constant.");
            southMarket.Exits.Add("west", 70);
            southMarket.Exits.Add("north", 78);
            southMarket.Exits.Add("east", 72);
            
            // Needs update
            Room stablesDistrict = CreateRoom(72, "stablesDistrict", "Belum - Stables District", "The smell of hay and horses is strong here. Several stables and paddocks house animals for travelers and merchants. A weathered sign advertises boarding rates.");
            stablesDistrict.Exits.Add("west", 71);
            stablesDistrict.Exits.Add("north", 73);

            // Needs update
            Room poorQuarter = CreateRoom(73, "poorQuarter", "Belum - Poor Quarter", "The buildings here are older and less maintained than elsewhere in town. Laundry hangs from windows, and children play in the narrow streets. The locals eye you with a mix of curiosity and wariness.");
            poorQuarter.Exits.Add("south", 72);
            poorQuarter.Exits.Add("north", 74);
            poorQuarter.Exits.Add("west", 78);

            // Needs update
            Room theGoldenGrape = CreateRoom(74, "theGoldenGrape", "Belum - The Golden Grape Tavern", "A large and welcoming tavern. The sign shows a golden bunch of grapes. Laughter and music spill out through the open door. This seems like the place to hear local news and rumors.");
            theGoldenGrape.Exits.Add("west", 77);
            theGoldenGrape.Exits.Add("south", 73);
            theGoldenGrape.Exits.Add("north", 75);
            theGoldenGrape.NPCs.Add(npcs["Barkeep"].Clone());

            // Needs update
            Room merchantRow = CreateRoom(75, "merchantRow", "Belum - Merchant Row", "Prosperous shops display their goods through glass windows - a luxury in these parts. Tailors, jewelers, and specialty craftsmen ply their trades here.");
            merchantRow.Exits.Add("west", 74);
            merchantRow.Exits.Add("south", 71);
            merchantRow.Exits.Add("north", 79);
            merchantRow.Exits.Add("east", 76);

            // Needs update
            Room craftsmansWay = CreateRoom(76, "craftsmansWay", "Belum - Craftsman's Way", "The ring of hammers on anvils echoes here. Blacksmiths, coopers, and carpenters work their trades. The air is warm from forge fires.");
            craftsmansWay.Exits.Add("west", 81);
            craftsmansWay.Exits.Add("south", 77);
            craftsmansWay.Exits.Add("east", 75);

            // Needs update
            Room backAlleys = CreateRoom(77, "backAlleys", "Belum - Back Alleys", "Narrow alleyways wind between cramped buildings. It's darker here, and the main bustle of the town feels far away. Not the safest part of town after dark.");
            backAlleys.Exits.Add("east", 74);
            backAlleys.Exits.Add("south", 78);
            backAlleys.Exits.Add("north", 76);
            backAlleys.Exits.Add("west", 80);

            // Needs update
            Room blacksmithForge = CreateRoom(78, "blacksmithForge", "Belum - The Iron Anvil", "A large smithy with multiple forges burning hot. Weapons, tools, and armor line the walls. The smith is a mountain of a man with arms like tree trunks.");
            blacksmithForge.Exits.Add("west", 79);
            blacksmithForge.Exits.Add("south", 76);
            blacksmithForge.Exits.Add("north", 84);
            blacksmithForge.NPCs.Add(npcs["Blacksmith"].Clone());

            // Needs update
            Room mainStreetSouth = CreateRoom(79, "mainStreetSouth", "Belum - Main Street (South)", "The main thoroughfare of Belum stretches north and south. Shops and taverns line both sides. A public fountain provides a gathering spot for locals.");
            mainStreetSouth.Exits.Add("south", 70);
            mainStreetSouth.Exits.Add("north", 80);
            mainStreetSouth.Exits.Add("east", 78);
            mainStreetSouth.Exits.Add("west", 84);
            mainStreetSouth.NPCs.Add(npcs["Villager"].Clone());

            // Needs update
            Room townSquare = CreateRoom(80, "townSquare", "Belum - Town Square", "The heart of Belum opens into a grand square. A large stone fountain depicting Neptune dominates the center. Important-looking buildings surround the square - the town hall, a temple, and what appears to be a guild house of some kind.");
            townSquare.Exits.Add("south", 79);
            townSquare.Exits.Add("north", 81);
            townSquare.Exits.Add("east", 77);
            townSquare.Exits.Add("west", 83);
            townSquare.NPCs.Add(npcs["Town Guard"].Clone());
            townSquare.NPCs.Add(npcs["Villager"].Clone());
            townSquare.NPCs.Add(npcs["Merchant"].Clone());

            // Needs update
            Room residentialNorth = CreateRoom(81, "residentialNorth", "Belum - North Residential", "Well-appointed homes belonging to Belum's more prosperous citizens line this quiet street. Gardens and small courtyards provide greenery.");
            residentialNorth.Exits.Add("south", 82);
            residentialNorth.Exits.Add("east", 87);


            // --- ROW 4 ---
            Room templeDistrict = CreateRoom(82, "templeDistrict", "Belum - Temple District", "Massive temples of white stone surround an open plaza.  The largest of all of these, to the north, is devoted to Keius - father of the gods.  If you look straight up, you can barely see the tops of the colossal columns that line the temple's facade.  Temples to major and minor gods of Keius' pantheon fill the rest of the square.  To the east is a residential district on the main street through Belum.  To the south are several inns, and more residences to the west.");
            templeDistrict.Exits.Add("south", 83);
            templeDistrict.Exits.Add("east", 81);
            templeDistrict.Exits.Add("west", 89);

            Room innDistrict = CreateRoom(83, "innDistrict", "Belum - Inn District", "Several inns compete for business here, their signs creaking in the breeze. 'The Wanderer's Rest', 'The Sleeping Lion', and 'Beds & Breakfast' all promise comfortable lodging.");
            innDistrict.Exits.Add("west", 82);
            innDistrict.Exits.Add("south", 79);
            innDistrict.Exits.Add("north", 87);
            innDistrict.Exits.Add("east", 84);

            Room armorersRow = CreateRoom(84, "armorersRow", "Belum - Armorer's Row", "Shops specializing in armor and protective gear line this street. Mannequins display everything from leather jerkins to full plate mail. A testing dummy stands outside one shop, heavily dented.");
            armorersRow.Exits.Add("west", 83);
            armorersRow.Exits.Add("south", 80);
            armorersRow.Exits.Add("north", 88);

            Room thievesGuild = CreateRoom(85, "thievesGuild", "Belum - Unmarked Alley", "A dead-end alley with a single unmarked door. Those who know, know. Those who don't, shouldn't be here.");
            thievesGuild.Exits.Add("east", 82);
            thievesGuild.Exits.Add("south", 81);

            Room shadowyCorner = CreateRoom(86, "shadowyCorner", "Belum - A Shadowy Corner", "This corner of town sees less traffic. A nondescript door leads to what might be a less-than-legitimate establishment. The locals here don't meet your eyes.");
            shadowyCorner.Exits.Add("east", 78);
            shadowyCorner.Exits.Add("south", 77);
            shadowyCorner.Exits.Add("north", 85);

            Room townHall = CreateRoom(87, "townHall", "Belum - Town Hall", "The administrative center of Belum. An imposing building with marble columns and bronze doors. Guards stand at attention. A notice board displays official proclamations.");
            townHall.Exits.Add("west", 86);
            townHall.Exits.Add("south", 83);
            townHall.Exits.Add("east", 88);
            townHall.NPCs.Add(npcs["Town Guard"].Clone());

            Room barracks = CreateRoom(88, "barracks", "Belum - Guard Barracks", "The town guard's headquarters. Soldiers drill in a courtyard while others sharpen weapons or play dice. The captain's office is visible through an open door.");
            barracks.Exits.Add("west", 87);
            barracks.Exits.Add("south", 84);
            barracks.Exits.Add("north", 89);
            barracks.NPCs.Add(npcs["Town Guard"].Clone());
            barracks.NPCs.Add(npcs["Town Guard"].Clone());

            Room room89 = CreateRoom(89, "room89", "Room 89", "This is room 89.  Inexplicably, there's nothing here.  Literally.  Just empty void as far as the eyes can see to the north and west.  To the south, you see the town guard's barracks.  To the east, you see an open plaza surrounded by majestic temples of polished white stone.");
            room89.Exits.Add("south", 88);
            room89.Exits.Add("east", 82);

            

            // Guild Hall
            rooms.Add(1, bedroom);
            rooms.Add(2, hallway);
            rooms.Add(3, study);
            rooms.Add(4, commonArea);
            rooms.Add(5, frontDoor);

            // Crossroads Area
            rooms.Add(6, guildPath);
            rooms.Add(7, theCrossRoads);
            rooms.Add(8, westPath);
            rooms.Add(9, westernBend);

            // Gaius' Farm
            rooms.Add(10, gaiusFarmFields);
            rooms.Add(11, gaiusFarmHouse);

            // Mountains
            rooms.Add(12, lowerSlopes);
            rooms.Add(13, mountainPath);
            rooms.Add(14, rockyOutcrop);
            rooms.Add(15, iceCavern);
            rooms.Add(16, mountainPeak);
            rooms.Add(17, ancientAltar);

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