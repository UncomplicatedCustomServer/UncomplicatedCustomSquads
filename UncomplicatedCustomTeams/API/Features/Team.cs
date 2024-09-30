﻿using Exiled.API.Features;
using Respawning;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UncomplicatedCustomRoles.API.Features;
using UncomplicatedCustomTeams.Utilities;
using UnityEngine;

namespace UncomplicatedCustomTeams.API.Features
{
    public class Team
    {
        /// <summary>
        /// Gets a complete list of every custom <see cref="Team"/> registered
        /// </summary>
        public static List<InternalTeam> List { get; } = new();

        /// <summary>
        /// Register a new custom <see cref="Team"/>
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public static bool Register(Team team)
        {
            foreach (EssentialCustomRole role in team.Roles)
                if (!CustomRole.TryGet(role.Id, out _))
                {
                    Log.Warn($"Failed to register custom team {team.Name}: Custom Role {role.Id} not found inside UCR!");
                    return false;
                }

            List.Add(team);
            return true;
        }

        internal static void SimpleRegister(Team team) => Register(team);

        /// <summary>
        /// Unregister a custom <see cref="Team"/>
        /// </summary>
        /// <param name="team"></param>
        public static void Unregister(Team team) => List.Remove(team);

        /// <summary>
        /// The Id of the custom <see cref="Team"/>
        /// </summary>
        [Description("The Id of the custom Team")]
        public uint Id { get; set; } = 1;

        /// <summary>
        /// The name of the custom <see cref="Team"/>
        /// </summary>
        public string Name { get; set; } = "GOC";

        /// <summary>
        /// The minimum number of players that are required to be on the server to make this custom <see cref="Team"/> spawn
        /// </summary>
        public int MinPlayers { get; set; } = 1;

        /// <summary>
        /// The chance of spawning of this custom <see cref="Team"/>.
        /// 0 is 0% and 100 is 100%!
        /// </summary>
        public uint SpawnChance { get; set; } = 100;

        /// <summary>
        /// The wave that will be replaced by this custom wave
        /// </summary>
        public SpawnableTeamType SpawnWave { get; set; } = SpawnableTeamType.NineTailedFox;

        /// <summary>
        /// The SpawnPosition of the wave.<br></br>
        /// If Vector3.zero or Vector3.one then it will be retrived from the RoleTypeId
        /// </summary>
        public Vector3 SpawnPosition { get; set; } = Vector3.zero;

        /// <summary>
        /// The cassie message that will be sent when the team spawn - empty to disable
        /// </summary>
        public string CassieMessage { get; set; } = "team arrived";

        /// <summary>
        /// The translation of the cassie message
        /// </summary>
        public string CassieTranslation { get; set; } = "Team arrived!";

        /// <summary>
        /// The list of every role that will be a part of this wave
        /// </summary>
        public List<EssentialCustomRole> Roles { get; set; } = new()
        {
            new()
            {
                MaxPlayers = 1,
            },
            new()
            {
                Id = 2,
                MaxPlayers = 500
            }
        };

        public static InternalTeam EvaluateSpawn(SpawnableTeamType wave)
        {
            List<InternalTeam> Teams = new();

            foreach (InternalTeam Team in List.Where(t => t.SpawnWave == wave))
                for (int a = 0; a < Team.SpawnChance; a++)
                    Teams.Add(Team);

            LogManager.Debug($"Evaluated team count, found {Teams.Count}/100 elements [{List.Where(t => t.SpawnWave == wave).Count()}]!\nIf the number is less than 100 THERE's A PROBLEM!");

            int Chance = new System.Random().Next(0, 99);
            if (Teams.Count > Chance)
                return Teams[Chance];

            return null;
        }
    }
}
