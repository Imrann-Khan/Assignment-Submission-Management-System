export interface LoginResult{
    token : string;
    userId : string;
    fullName: string;
    email: string;
    role: 'Admin' | 'Teacher' | 'Student';
}