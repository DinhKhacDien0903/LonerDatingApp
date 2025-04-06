namespace LonerApp.Helpers.Extensions
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this InterestEnum interest)
        {
            return interest switch
            {
                InterestEnum.Am_nhac => "Âm nhạc",
                InterestEnum.The_thao => "Thể thao",
                InterestEnum.Du_lich => "Du lịch",
                InterestEnum.Nau_an => "Nấu ăn",
                InterestEnum.Lap_trinh => "Lập trình",
                InterestEnum.Chup_anh => "Chụp ảnh",
                InterestEnum.Ve_tranh => "Vẽ tranh",
                InterestEnum.Doc_sach => "Đọc sách",
                InterestEnum.Xem_phim => "Xem phim",
                InterestEnum.Choi_game => "Chơi game",
                InterestEnum.Viet_lach => "Viết lách",
                InterestEnum.Hoc_ngoai_ngu => "Học ngoại ngữ",
                InterestEnum.Di_bo => "Đi bộ",
                InterestEnum.Yoga => "Yoga",
                InterestEnum.Thoi_trang => "Thời trang",
                InterestEnum.Cau_ca => "Câu cá",
                InterestEnum.Cam_trai => "Cắm trại",
                InterestEnum.Khieu_vu => "Khiêu vũ",
                InterestEnum.Lam_vuon => "Làm vườn",
                InterestEnum.Choi_nhac_cu => "Chơi nhạc cụ",
                _ => interest.ToString()
            };
        }
    }
}
