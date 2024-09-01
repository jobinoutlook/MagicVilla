using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> villaList = new List<VillaDTO>
            {
                new VillaDTO { Id=1, Name="Pool Villa", Occupancy=4,Sqft=100 },
                new VillaDTO { Id=2,Name="Beach Villa", Occupancy=3,Sqft=300 }
            };
    }
}
