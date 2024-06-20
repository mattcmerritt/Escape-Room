using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: May want to have user input this, so that clients can input a database string.
// Currently requires the database connection string to be included in the build.
// Solution to protect the database connection strings.

[CreateAssetMenu(fileName = "New Secret")]
public class Secret : ScriptableObject
{
    public string Content;
}