export type SubmissionStatus = "Submitted" | "Graded" | "Returned";

export interface SubmissionDto {
  id: string;
  assignmentId: string;
  assignmentTitle: string;
  maxMarks: number;
  deadline: string;
  studentId: string;
  studentName: string;
  answerText: string;
  submittedAt: string;
  status: SubmissionStatus;
  marks: number | null;
  feedback: string | null;
  gradedAt: string | null;
  gradedByName: string | null;
}
