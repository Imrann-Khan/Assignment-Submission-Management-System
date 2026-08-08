export interface SubjectDto {
    id: string;
    name: string;
    classId: string;
}

export interface ClassDto {
    id: string;
    name: string;
    studentCount: number;
    subjects: SubjectDto[];
}