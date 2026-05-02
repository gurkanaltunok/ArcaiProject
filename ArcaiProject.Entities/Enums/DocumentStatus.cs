namespace ArcaiProject.Entities.Enums
{
    /// <summary>
    /// Belgenin mevcut durumunu temsil eder
    /// </summary>
    public enum DocumentStatus
    {
        /// <summary>
        /// Arşivde, ödünç alınabilir
        /// </summary>
        Available,
        
        /// <summary>
        /// Ödünç verilmiş
        /// </summary>
        CheckedOut,
        
        /// <summary>
        /// Kayıp
        /// </summary>
        Missing
    }
}

