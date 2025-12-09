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
            study.Items.Add("iron sword");

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
                    {"iron sword", 5}
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
                    {"iron sword", 10}
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
            forestBend.NPCs.Add(npcs["Dire Wolf"].Clone());  // Clone dire wolf
            forestBend.NPCs.Add(npcs["Bandit"].Clone());  // Clone bandit  
            forestBend.NPCs.Add(npcs["Bandit Thug"].Clone());  // Clone bandit thug
            forestBend.CanRespawn = true;
            forestBend.RespawnTimeHours = 16f;
            forestBend.OriginalNPCs.Add(npcs["Dire Wolf"].Clone());
            forestBend.OriginalNPCs.Add(npcs["Bandit"].Clone());
            forestBend.OriginalNPCs.Add(npcs["Bandit Thug"].Clone());

            // NEW MOUNTAIN PATH ROOMS
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
            abandonedCampsite.Items.Add("chest");

            rooms.Add(1, bedroom);
            rooms.Add(2, hallway);
            rooms.Add(3, study);
            rooms.Add(4, commonArea);
            rooms.Add(5, frontDoor);
            rooms.Add(6, guildPath);
            rooms.Add(7, theCrossRoads);
            rooms.Add(8, westPath);
            rooms.Add(9, westernBend);
            rooms.Add(10, gaiusFarmFields);
            rooms.Add(11, gaiusFarmHouse);
            rooms.Add(12, lowerSlopes);
            rooms.Add(13, mountainPath);
            rooms.Add(14, rockyOutcrop);
            rooms.Add(15, iceCavern);
            rooms.Add(16, mountainPeak);
            rooms.Add(17, ancientAltar);
            rooms.Add(40, eastPath);
            rooms.Add(41, woodedPath);
            rooms.Add(42, pathWithCart);
            rooms.Add(43, nearFallenTree);
            rooms.Add(44, deepForest);
            rooms.Add(45, forestStream);
            rooms.Add(46, forestBend);
            rooms.Add(47, abandonedCampsite);

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