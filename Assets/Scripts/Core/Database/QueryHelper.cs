using Npgsql;
using System;
using UnityEngine;

public class QueryHelper
{

    // Veritabaný sorgusu çalýþtýrma (UPDATE, INSERT, DELETE gibi)
    public bool ExecuteQuery(string query, NpgsqlConnection connection, params NpgsqlParameter[] parameters)
    {
        try
        {
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddRange(parameters); // Parametreleri ekle
                cmd.ExecuteNonQuery(); // Sorguyu çalýþtýr
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sorgu hatasý: {ex.Message}");

            return false;
        }
    }
    
    public bool ExecuteNonQuery(string query, NpgsqlConnection connection)
    {
        try
        {
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.ExecuteNonQuery(); // Sorguyu çalýþtýr
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sorgu hatasý: {ex.Message}");

            return false;
        }
    }

    // Veritabanýndan birden fazla satýr döndüren sorgu
    public NpgsqlDataReader ExecuteReader(string query, NpgsqlConnection connection, params NpgsqlParameter[] parameters)
    {
        try
        {
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddRange(parameters); // Parametreleri ekle
            return cmd.ExecuteReader(); // Veri okuyucu döndür
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sorgu hatasý: {ex.Message}");
            return null;
        }
    }

    public NpgsqlDataReader ExecuteReaderWithoutParametres(string query, NpgsqlConnection connection)
    {
        try
        {
            var cmd = new NpgsqlCommand(query, connection);
            return cmd.ExecuteReader(); // Veri okuyucu döndür
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sorgu hatasý: {ex.Message}");
            return null;
        }
    }

    // Veritabanýndan bir satýr döndüren sorgu
    public NpgsqlDataReader ExecuteQuerySingle(string query, NpgsqlConnection connection, params NpgsqlParameter[] parameters)
    {
        try
        {
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddRange(parameters); // Parametreleri ekle
            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return reader; // Ýlk satýrý döndür
            }
            return null; // Eðer sonuç yoksa null döndür
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sorgu hatasý: {ex.Message}");
            return null;
        }
    }
}
