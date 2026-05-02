namespace ArcaiProject.Entities.Enums
{
    /// <summary>
    /// Ödünç alma kaydının durumunu temsil eder
    /// </summary>
    public enum BorrowingRecordStatus
    {
        /// <summary>
        /// Öğretmen talep etti, onay bekliyor
        /// </summary>
        Pending,
        
        /// <summary>
        /// Sekreter onayladı, teslim alınmayı bekliyor
        /// </summary>
        Approved,
        
        /// <summary>
        /// Sekreter talebi reddetti
        /// </summary>
        Rejected,
        
        /// <summary>
        /// Belge fiziksel olarak teslim edildi
        /// </summary>
        CheckedOut,
        
        /// <summary>
        /// Belge iade edildi
        /// </summary>
        Returned,
        
        /// <summary>
        /// Süresi geçti, henüz iade edilmedi
        /// </summary>
        Overdue
    }
}

