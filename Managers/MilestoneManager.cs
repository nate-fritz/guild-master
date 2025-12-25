using System;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;

namespace GuildMaster.Managers
{
    public class MilestoneManager
    {
        private GameContext gameContext;
        private MessageManager messageManager;

        public MilestoneManager(GameContext context, MessageManager msgManager)
        {
            gameContext = context;
            messageManager = msgManager;
        }

        public void CheckMilestones()
        {
            if (gameContext?.Player == null)
                return;

            var player = gameContext.Player;

            // Check recruit count milestones
            CheckRecruitMilestones(player);
        }

        private void CheckRecruitMilestones(Player player)
        {
            int totalRecruits = player.Recruits.Count;

            // Milestone: 3rd recruit - Guild expansion
            if (totalRecruits >= 3 && !gameContext.CompletedMilestones.Contains("guild_expansion_1"))
            {
                gameContext.CompletedMilestones.Add("guild_expansion_1");

                // Trigger narrative message
                messageManager?.CheckAndShowMessage("guild_expansion_1");

                // Could unlock new room states here
                // Example: gameContext.RoomStateOverrides[1] = "expanded";
            }

            // Milestone: 5th recruit - Imperial visitor
            if (totalRecruits >= 5 && !gameContext.CompletedMilestones.Contains("imperial_visitor"))
            {
                gameContext.CompletedMilestones.Add("imperial_visitor");

                // Trigger narrative message
                messageManager?.CheckAndShowMessage("imperial_visitor");

                // Could spawn new NPC or change room state
            }

            // Update total recruits ever counter
            if (totalRecruits > gameContext.TotalRecruitsEver)
            {
                gameContext.TotalRecruitsEver = totalRecruits;
            }
        }

        public string GetRoomState(int roomId)
        {
            if (gameContext.RoomStateOverrides.ContainsKey(roomId))
            {
                return gameContext.RoomStateOverrides[roomId];
            }
            return "default";
        }

        public void SetRoomState(int roomId, string state)
        {
            gameContext.RoomStateOverrides[roomId] = state;
        }
    }
}
