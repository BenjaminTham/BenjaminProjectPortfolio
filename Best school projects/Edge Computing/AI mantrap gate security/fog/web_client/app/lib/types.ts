
// Response payloads
export type ApiResponse<T> = {
  status: number;
  body: T | {message: string};
}

export type Login = {
  message: string,
  data: {id: number, username: string, role: string}
}

export type Logout = {
  message: string
}

export type GetUsers = {
  errors?: {[index: string]: string}[],
  message?: string,
  data: {id: number, username: string, email: string, role: string, biometrics_enrolled: boolean, alert_subscribed: boolean, account_locked: boolean, location: string}[]
}

export type AddUser = {
  errors?: {[index: string]: string}[],
  message: string
}

export type UpdateUser = {
  errors?: {[index: string]: string}[],
  message: string
}

export type DeleteUser = {
  message: string
}

export type GetMantraps = {
  errors?: {[index: string]: string}[],
  message?: string,
  data: {id: number, location: string, token: string, entry_gate_status: string, exit_gate_status: string}[]
}

export type AddMantrap = {
  errors?: {[index: string]: string}[],
  message: string
}

export type UpdateMantrap = {
  errors?: {[index: string]: string}[],
  message: string
}

export type DeleteMantrap = {
  message: string
}

export type CommandDoor = {
  message: string
}

export type EnrollBiometrics = {
  errors?: {[index: string]: string}[],
  message: string
}

export type DeleteBiometrics = {
  message: string
}

export type GetLogs = {
  errors?: {[index: string]: string}[],
  message?: string,
  data: {id: number, category: string, user_id: string, mantrap_id: string, timestamp: string, file: string}[]
}

// Request payloads
export type RequestBody = LoginRequest | GetUsersRequest | AddUserRequest | UpdateUserRequest | GetMantrapsRequest | AddMantrapRequest | UpdateMantrapRequest | GetLogsRequest;

export type LoginRequest = {
  username: string,
  password: string
}

export type GetUsersRequest = {
  id?: number
  username?: string,
  email?: string,
  role?:string
}

export type AddUserRequest = {
  username: string,
  email: string,
  password: string,
  role: string,
  alert_subscribed: boolean
}

export type UpdateUserRequest = {
  username: string,
  email: string,
  password: string,
  role: string,
  alert_subscribed: boolean,
  unlock_account: boolean
}

export type GetMantrapsRequest = {
  id?: number,
  location?: string
}

export type AddMantrapRequest = {
  location: string
}

export type UpdateMantrapRequest = {
  location: string
}

export type GetLogsRequest = {
  category?: string,
  user_id?: string,
  mantrap_id?: string
}








