export interface NotificationDto {
  id: string;
  type: string;
  message: string;
  isRead: boolean;
  createdAt: string;
  relatedAssignmentId: string | null;
}
