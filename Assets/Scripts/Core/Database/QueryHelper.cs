using Npgsql;
using System;
using UnityEngine;

public class QueryHelper
{

    // Veritaban� sorgusu �al��t�rma (UPDATE, INSERT, DELETE gibi)
    public bool ExecuteQuery(string query, NpgsqlConnection connection, params NpgsqlParameter[] parameters)
    {
        try
        {
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddRange(parameters); // Parametreleri ekle
                cmd.ExecuteNonQuery(); // Sorguyu �al��t�r
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sorgu hatas�: {ex.Message}");

            return false;
        }
    }
    
    public bool ExecuteNonQuery(string query, NpgsqlConnection connection)
    {
        try
        {
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.ExecuteNonQuery(); // Sorguyu �al��t�r
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sorgu hatas�: {ex.Message}");

            return false;
        }
    }

    // Veritaban�ndan birden fazla sat�r d�nd�ren sorgu
    public NpgsqlDataReader ExecuteReader(string query, NpgsqlConnection connection, params NpgsqlParameter[] parameters)
    {
        try
        {
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddRange(parameters); // Parametreleri ekle
            return cmd.ExecuteReader(); // Veri okuyucu d�nd�r
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sorgu hatas�: {ex.Message}");
            return null;
        }
    }

    public NpgsqlDataReader ExecuteReaderWithoutParametres(string query, NpgsqlConnection connection)
    {
        try
        {
            var cmd = new NpgsqlCommand(query, connection);
            return cmd.ExecuteReader(); // Veri okuyucu d�nd�r
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sorgu hatas�: {ex.Message}");
            return null;
        }
    }

    // Veritaban�ndan bir sat�r d�nd�ren sorgu
    public NpgsqlDataReader ExecuteQuerySingle(string query, NpgsqlConnection connection, params NpgsqlParameter[] parameters)
    {
        try
        {
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddRange(parameters); // Parametreleri ekle
            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return reader; // �lk sat�r� d�nd�r
            }
            return null; // E�er sonu� yoksa null d�nd�r
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sorgu hatas�: {ex.Message}");
            return null;
        }
    }
}
