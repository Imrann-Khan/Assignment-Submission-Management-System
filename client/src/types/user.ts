export type UserRole = 'Admin' | 'Teacher' | 'Student';


export interface UserDto {
    id: string;
    fullName: string;
    email: string;
    role: UserRole;
    isActive: boolean;
    classId: string | null;
    className: string | null;
}