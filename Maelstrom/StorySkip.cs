﻿using System;
using System.IO;
using FF8Mod.Field;
using FF8Mod.Archive;

namespace FF8Mod.Maelstrom
{
    public static class StorySkip
    {
        public static BinaryPatch IntroPatch = new BinaryPatch(0x273fb, new byte[] { 0x33, 0x30 }, new byte[] { 0x30, 0x31 });

        public static void Apply(FileSource fieldSource, string af3dnPath)
        {
            // replace liberi fatali intro with quistis walking through a door
            ImportScript(fieldSource, "start0", 0, 1);
            IntroPatch.Apply(af3dnPath);

            // brief conversation in the infirmary, receive 2 GFs and a party member
            ImportScript(fieldSource, "bghoke_2", 12, 1);
            ImportScript(fieldSource, "bghoke_2", 6, 4);

            // tutorial at the front gate
            DeleteEntity(fieldSource, "bggate_1", 0);

            // fire cavern tutorials & dialogue with quistis
            DeleteEntity(fieldSource, "bdview1", 0);

            // faculty fellas at the cave entrance
            DeleteEntity(fieldSource, "bdview1", 11);
            DeleteEntity(fieldSource, "bdview1", 12);
            DeleteEntity(fieldSource, "bdenter1", 9);
            DeleteEntity(fieldSource, "bdenter1", 10);

            // fire cavern dialogue & timers
            DeleteScript(fieldSource, "bdin1", 11, 1);
            DeleteScript(fieldSource, "bdin2", 11, 1);
            DeleteScript(fieldSource, "bdin3", 11, 1);
            DeleteScript(fieldSource, "bdin4", 12, 1);
            DeleteScript(fieldSource, "bdin5", 15, 1);
            DeleteEntity(fieldSource, "bdin5", 0);
            DeleteScript(fieldSource, "bdifrit1", 15, 1);
            ImportScript(fieldSource, "bdifrit1", 0, 5);
            ImportScript(fieldSource, "bdifrit1", 14, 4);
        }

        public static void Remove(string af3dnPath)
        {
            // todo: reset field scripts
            IntroPatch.Remove(af3dnPath);
        }

        // overwrite a field script with one imported from a text file
        public static void ImportScript(FileSource fieldSource, string fieldName, int entity, int script, string importPath)
        {
            var field = FieldScript.FromSource(fieldSource, fieldName);
            field.ReplaceScript(entity, script, File.ReadAllText(importPath));
            SaveToSource(fieldSource, fieldName, field.Encode());
        }

        // slightly easier import with the filename convention "fieldName.entityID.scriptID.txt"
        public static void ImportScript(FileSource fieldSource, string fieldName, int entity, int script)
        {
            ImportScript(fieldSource, fieldName, entity, script, string.Format(@"FieldScripts\{0}.{1}.{2}.txt", fieldName, entity, script));
        }

        // return a field script to its original state
        public static void ResetScript(FileSource fieldSource, string fieldName, int entity, int script)
        {
            ImportScript(fieldSource, fieldName, entity, script, string.Format(@"OrigFieldScripts\{0}.{1}.{2}.txt", fieldName, entity, script));
        }

        // delete a single script
        public static void DeleteScript(FileSource fieldSource, string fieldName, int entity, int script)
        {
            var field = FieldScript.FromSource(fieldSource, fieldName);
            var label = field.Entities[entity].Scripts[script].Instructions[0].Param;
            var nullScript = string.Format("lbl {0}{1}ret 8", label, Environment.NewLine);
            field.ReplaceScript(entity, script, nullScript);
            SaveToSource(fieldSource, fieldName, field.Encode());
        }

        // remove an entity from the field by deleting its initialisation script
        public static void DeleteEntity(FileSource fieldSource, string fieldName, int entity)
        {
            DeleteScript(fieldSource, fieldName, entity, 0);
        }

        private static void SaveToSource(FileSource fieldSource, string fieldName, byte[] fieldCode)
        {
            var innerSource = new FileSource(FieldScript.GetFieldPath(fieldName), fieldSource);
            innerSource.ReplaceFile(FieldScript.GetFieldPath(fieldName) + "\\" + fieldName + ".jsm", fieldCode);
        }
    }
}