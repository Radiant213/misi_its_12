using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;


namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class KategoriController : ControllerBase
    {
        private readonly Koneksi _koneksi;
        public KategoriController(Koneksi koneksi)
        {
            _koneksi = koneksi;
        }



        // Endpoint default
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = new List<object>();
            using (SqlConnection conn = _koneksi.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Kategori", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new

                    {
                        Id = reader["Id"],
                        NamaKategori = reader["NamaKategori"]

                    });
                }
                reader.Close();
            }
            return Ok(list);
        }

        [HttpGet("{id}")]
        public object Reader(int id)
        {

            var result = new
            {
                Status = 200,
                
                Data = new
                {
                    Id = 0,
                    Nama = ""
                }
            };
            
            SqlConnection connection = _koneksi.GetConnection();

            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Kategori WHERE Id = " + id, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    result = new
                    {
                        Status = 200,
                        Data = new
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nama = reader["NamaKategori"].ToString()
                        }
                    };
                };

                return result;
            }

            catch (Exception e)
            {
                var errorResult = new
                {
                    Status = 500,
                    Message = "Koneksi Gagal Karena: " + e.Message
                };
                return errorResult;
            }

        }

            // Endpoint "NamaKategori" ini tuh contoh yang BENAR/AMAN
            // - Menggunakan parameter SQL (@nama) sehingga input user tidak langsung disisipkan ke query.
            // - Parameter diberikan tipe dan ukuran eksplisit untuk menghindari tebakan tipe yang buruk.
            // Keuntungan: mencegah SQL Injection, rencana eksekusi lebih konsisten.
            // Endpoint NamaKategori yang lebih bagus
            [HttpGet("NamaKategori")]
        public IActionResult GetByNamaAddWithValue([FromQuery] string nama)
        {
            if (string.IsNullOrWhiteSpace(nama))
                return BadRequest("Parameter 'nama' wajib diisi.");

            var list = new List<object>();
            using (SqlConnection conn = _koneksi.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT Id, NamaKategori FROM Kategori WHERE NamaKategori = @nama", conn))
                {
                    cmd.Parameters.AddWithValue("@nama", nama ?? (object)DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new
                            {
                                Id = reader["Id"],
                                NamaKategori = reader["NamaKategori"]
                            });
                        }
                    }
                }
            }
            return Ok(list);
        }

        // Endpoint "NamaKategoriLemah" ini itu contoh yang LEMAH / TIDAK AMAN
        // - Membuat query dengan menggabungkan langsung input user ke string SQL (concatenation).
        // - RENTAN terhadap SQL Injection. Contoh eksploitasi: jika user mengirim "Makanan' OR '1'='1"
        //   maka query menjadi: SELECT ... WHERE NamaKategori = 'Makanan' OR '1'='1' -> mengembalikan semua baris.
        [HttpGet("NamaKategoriLemah")]
        public IActionResult GetByNama_Lemah([FromQuery] string nama)
        {
            if (string.IsNullOrWhiteSpace(nama))
                return BadRequest("Parameter 'nama' wajib diisi.");

            var list = new List<object>();
            using (SqlConnection conn = _koneksi.GetConnection())
            {
                conn.Open();

                // INI CONTOH TIDAK AMAN: memasukkan input langsung ke query
                string query = "SELECT Id, NamaKategori FROM Kategori WHERE NamaKategori = '" + nama + "'";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new
                            {
                                Id = reader["Id"],
                                NamaKategori = reader["NamaKategori"]
                            });
                        }
                    }
                }
            }
            return Ok(list);
        }
    }
}


