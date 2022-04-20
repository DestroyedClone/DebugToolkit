﻿using RoR2;
using System.Text;
using static DebugToolkit.Log;
using UnityEngine;

namespace DebugToolkit.Commands
{
    class Lists
    {
        [ConCommand(commandName = "list_interactables", flags = ConVarFlags.None, helpText = "Lists all interactables.")]
        private static void CCList_interactables(ConCommandArgs _)
        {
            StringBuilder s = new StringBuilder();
            foreach (InteractableSpawnCard isc in StringFinder.Instance.InteractableSpawnCards)
            {
                s.AppendLine(isc.name.Replace("isc", string.Empty));
            }
            Log.Message(s.ToString(), LogLevel.MessageClientOnly);
        }

        [ConCommand(commandName = "list_player", flags = ConVarFlags.None, helpText = Lang.LISTPLAYER_ARGS)]
        private static void CCListPlayer(ConCommandArgs args)
        {
            NetworkUser n;
            StringBuilder list = new StringBuilder();
            for (int i = 0; i < NetworkUser.readOnlyInstancesList.Count; i++)
            {
                n = NetworkUser.readOnlyInstancesList[i];
                list.AppendLine($"[{i}]{n.userName}");

            }
            Log.MessageNetworked(list.ToString(), args, LogLevel.MessageClientOnly);
        }

        [ConCommand(commandName = "list_AI", flags = ConVarFlags.None, helpText = Lang.LISTAI_ARGS)]
        private static void CCListAI(ConCommandArgs _)
        {
            string langInvar;
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var master in MasterCatalog.allAiMasters)
            {
                langInvar = StringFinder.GetLangInvar(master.bodyPrefab.GetComponent<CharacterBody>().baseNameToken);
                sb.AppendLine($"[{i}]{master.name}={langInvar}");
                i++;
            }
            Log.Message(sb);
        }

        [ConCommand(commandName = "list_Body", flags = ConVarFlags.None, helpText = Lang.LISTBODY_ARGS)]
        private static void CCListBody(ConCommandArgs _)
        {
            string langInvar;
            int i = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                langInvar = StringFinder.GetLangInvar(body.baseNameToken);
                sb.AppendLine($"[{i}]{body.name}={langInvar}");
                i++;
            }
            Log.Message(sb);
        }

        [ConCommand(commandName = "list_Directorcards", flags = ConVarFlags.None, helpText = Lang.NOMESSAGE)]
        private static void CCListDirectorCards(ConCommandArgs _)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var card in StringFinder.Instance.DirectorCards)
            {
                sb.AppendLine($"{card.spawnCard.name}");
            }
            Log.Message(sb);
        }

        [ConCommand(commandName = "list_skin", flags = ConVarFlags.None, helpText = "List all bodies with skins" + Lang.LISTBODY_ARGS)]
        private static void CCListSkin(ConCommandArgs args)
        {
            string langInvar;
            StringBuilder sb = new StringBuilder();
            string bodyName = null;
            if (args.Count == 0)
            {
                args.userArgs.Add(Lang.ALL); //simple
            }
            if (args.Count >= 1)
            {
                bodyName = args.GetArgString(0);
                string upperBodyName = bodyName.ToUpperInvariant();

                switch (upperBodyName)
                {
                    case Lang.ALL:
                        foreach (var bodyComponent in BodyCatalog.allBodyPrefabBodyBodyComponents)
                        {
                            AppendSkinIndices(ref sb, bodyComponent);
                        }
                        break;
                    default:
                        CharacterBody body = null;
                        if (upperBodyName == "SELF")
                        {
                            if (args.sender == null)
                            {
                                Log.Message("Can't choose self if not in-game!", LogLevel.Error);
                                return;
                            }
                            if (args.senderBody)
                            {
                                body = args.senderBody;
                            }
                            else
                            {
                                if (args.senderMaster && args.senderMaster.bodyPrefab)
                                {
                                    body = args.senderMaster.bodyPrefab.GetComponent<CharacterBody>();
                                }
                                else
                                {
                                    body = BodyCatalog.GetBodyPrefabBodyComponent(args.sender.bodyIndexPreference);
                                    // a little redundant
                                }
                            }
                        }
                        else
                        {
                            string requestedBodyName = StringFinder.Instance.GetBodyName(args[0]);
                            if (requestedBodyName == null)
                            {
                                Log.MessageNetworked("Please use list_body to print CharacterBodies", args, LogLevel.MessageClientOnly);
                                return;
                            }
                            GameObject newBody = BodyCatalog.FindBodyPrefab(requestedBodyName);
                            body = newBody.GetComponent<CharacterBody>();
                        }
                        if (body)
                        {
                            AppendSkinIndices(ref sb, body);
                        } else
                        {
                            Log.MessageNetworked("Please use list_body to print CharacterBodies", args, LogLevel.MessageClientOnly);
                            return;
                        }
                        break;
                }
            }

            Log.Message(sb);
        }

        private static void AppendSkinIndices(ref StringBuilder stringBuilder, CharacterBody body)
        {
            var skins = BodyCatalog.GetBodySkins(body.bodyIndex);
            if (skins.Length > 0)
            {
                var langInvar = StringFinder.GetLangInvar(body.baseNameToken);
                stringBuilder.AppendLine($"[{body.bodyIndex}]{body.name}={langInvar}");
                int i = 0;
                foreach (var skinDef in skins)
                {
                    langInvar = StringFinder.GetLangInvar(skinDef.nameToken);
                    stringBuilder.AppendLine($"-[{i}={skinDef.skinIndex}] {skinDef.name}={langInvar}");
                    i++;
                }
            }
        }
    }
}
