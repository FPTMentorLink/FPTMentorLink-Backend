namespace Services.Utils;

public class Constants
{
    // Project constants
    public const int MaxProjectStudents = 5;
    public const int MinProjectStudents = 4;
    
    // Checkpoint constants
    public const int MaxCheckpoints = 4;
    
    // Account constants
    public const int MaxImportAccounts = 300;
    public const long MaxFileSize = 3 * 1024 * 1024; // 3MB = 3 * 1024KB * 1024byte
    
    // Deposit constants
    public const int MinDepositAmount = 10000; // 10,000 VND
    public const int MaxDepositAmount = 10000000; // 10,000,000 VND
    
    // Appointment constants
    // Tạo lịch hẹn phải trước ít nhất 48 giờ (2 ngày)
    public const int RequireCreateAppointmentInAdvance = 48;
    // Hủy lịch hẹn phải trước ít nhất 24 giờ (Dùng cho cancel và reject)
    public const int RequireCancelAppointmentInAdvance = 24; // 24 hours
    public const int MinAppointmentLength = 30; // 30 minutes
}