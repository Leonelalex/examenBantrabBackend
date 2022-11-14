using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Buffers.Text;

namespace examenApiBackend.DTO
{
    public partial class ImagenDTO
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string base64 { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public partial class Imagen
    {
        public string nombre { get; set; }
        public string base64 { get; set; }
    }
}
