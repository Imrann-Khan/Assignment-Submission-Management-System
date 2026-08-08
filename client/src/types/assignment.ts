export type AssignmentStatus = "Draft" | "Published";

export interface AssignmentDto {
  id: string;
  title: string;
  description: string;
  deadline: string;
  maxMarks: number;
  status: AssignmentStatus;
  classId: string;
  className: string;
  subjectId: string;
  subjectName: string;
  teacherId: string;
  teacherName: string;
  createdAt: string;
}
