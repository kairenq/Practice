namespace AdmissionSystem.Models;

public enum ApplicationStatus
{
    Draft,          // Черновик
    Submitted,      // Подана
    UnderReview,    // На рассмотрении
    Accepted,       // Принята
    Rejected,       // Отклонена
    Withdrawn       // Отозвана
}
