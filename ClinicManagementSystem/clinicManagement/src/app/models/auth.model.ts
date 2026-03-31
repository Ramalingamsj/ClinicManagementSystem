export interface LoginResponse {
  userId: number;
  username: string;
  fullName: string;
  roleId: number;
  roleName: string;
  token: string;
}

export interface User {
  userId: number;
  username: string;
  fullName: string;
  roleId: number;
  roleName?: string;
  contact?: string;
  email?: string;
}
