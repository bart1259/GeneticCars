using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

[CustomEditor(typeof(SimulationManager))]
public class SimManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SimulationManager simManager = ((SimulationManager)target);

        string path = EditorGUILayout.TextField("Path: ","Assets/_GameAssets/brains.txt");
        if (GUILayout.Button("Save Top Brain"))
        {
            File.Delete(path);
            float[][] dna = simManager.lastRoundDNA[simManager.lastRoundDNA.Count - 1];
            StreamWriter writer = new StreamWriter(path, true);
            //There is only one brain
            writer.Write("1;");
            writer.Write(Environment.NewLine);
            WriteDNA(ref writer, dna);
            writer.Close();

        }

        string top = EditorGUILayout.TextField("X= ","5");
        if (GUILayout.Button("Save Top X Brains"))
        {
            int topNumber;
            if (!int.TryParse(top,out topNumber))
            {
                Debug.LogError("Could not parse X: " + top);
                return;
            }

            File.Delete(path);
            StreamWriter writer = new StreamWriter(path, true);
            writer.Write(topNumber + ";");
            writer.Write(Environment.NewLine);
            for (int i = simManager.lastRoundDNA.Count - topNumber; i < simManager.lastRoundDNA.Count; i++)
            {
                float[][] dna = simManager.lastRoundDNA[i];
                WriteDNA(ref writer, dna);
            }
            writer.Close();

        }

        if (GUILayout.Button("Save All Brains"))
        {
            File.Delete(path);
            StreamWriter writer = new StreamWriter(path, true);
            writer.Write(simManager.lastRoundDNA.Count + ";");
            writer.Write(Environment.NewLine);
            for (int i = 0; i < simManager.lastRoundDNA.Count; i++)
            {
                float[][] dna = simManager.lastRoundDNA[i];
                WriteDNA(ref writer, dna);
            }
            writer.Close();
        }
    }

    //writes dna to a file
    private void WriteDNA(ref StreamWriter writer, float[][] dna)
    {
        int[] structure = GetStructure(dna);
        writer.Write(structure.Length);
        //Write structure
        for (int i = 0; i < structure.Length; i++)
        {
            writer.Write(";");
            writer.Write(structure[i]);
        }
        writer.Write(Environment.NewLine);
        //Write actual brain values
        for (int i = 0; i < structure.Length; i++)
        {
            for (int j = 0; j < dna[i].Length; j++)
            {
                if (j != 0)
                    writer.Write(";");
                writer.Write(dna[i][j]);
            }
            writer.Write(Environment.NewLine);
        }
    }

    private int[] GetStructure(float[][] sampleDNA)
    {
        int[] structure = new int[sampleDNA.Length];
        for (int i = 0; i < sampleDNA.Length; i++)
        {
            structure[i] = sampleDNA[i].Length;
        }
        return structure;
    }

}
