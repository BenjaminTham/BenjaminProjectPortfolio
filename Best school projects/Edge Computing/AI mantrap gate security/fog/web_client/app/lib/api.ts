import {
  AddMantrap,
  AddMantrapRequest,
  AddUser,
  AddUserRequest,
  ApiResponse,
  CommandDoor,
  DeleteBiometrics,
  DeleteMantrap,
  DeleteUser, EnrollBiometrics,
  GetLogs,
  GetLogsRequest,
  GetMantraps,
  GetMantrapsRequest,
  GetUsers,
  GetUsersRequest,
  Login,
  LoginRequest,
  Logout,
  RequestBody,
  UpdateMantrap,
  UpdateMantrapRequest,
  UpdateUser,
  UpdateUserRequest
} from "@/app/lib/types";

export const getData = async (key: string) => {
  // Server side rendering
  if (typeof window === "undefined") {
    const headers = await import("next/headers");
    const cookie = await headers.cookies();
    return JSON.parse(cookie.get(key)?.value ?? "null");
  }
  // Client side rendering
  const cookies = await import("js-cookie");
  return JSON.parse(cookies.default.get(key) ?? "null");
}

export const setData = async (key: string, value: object | string) => {
  const {default: Cookies} = await import("js-cookie")
  Cookies.set(key, JSON.stringify(value))
}

const deleteData = async (key: string) => {
  const {default: Cookies} = await import("js-cookie")
  Cookies.remove(key)
}

const sendRequest = async <T>(url: string, method = "GET", body: RequestBody = {}, formData: FormData | null = null): Promise<ApiResponse<T>> => {
  return await fetch(process.env.NEXT_PUBLIC_API_URL + url, {
    method: method,
    credentials: "include",
    headers: formData ? {}: {"Content-Type": "application/json"},
    ...(method !== "GET" ? {body: (formData ? formData : JSON.stringify(body))}: {})
  }).then(async response => {
    return {status: response.status, body: await response.json()}
  }).catch(error => {
    console.log(error)
    return {status: -1, body: {message: `Connection to ${url} failed`}}
  })
}

export const login = async (credentials: LoginRequest) => {
  const response = await sendRequest<Login>("/auth/login", "POST", credentials)

  // Login failed
  if (response.status !== 200)
    return response

  const data = (response.body as Login).data

  // Save user data
  await setData("user", data)

  return response
}

export const logout = async () => {
  await sendRequest<Logout>("/auth/logout");
  await deleteData("user")
}

export const getUsers = async (data: GetUsersRequest) => {
  return await sendRequest<GetUsers>("/user/list", "POST", data)
}

export const addUser = async (data: AddUserRequest) => {
  return await sendRequest<AddUser>("/user/add", "POST", data)
}

export const updateUser = async (userId: number, data: Partial<UpdateUserRequest>) => {
  return await sendRequest<UpdateUser>(`/user/update/${userId}`, "POST", data)
}

export const deleteUser = async (userId: number) => {
  return sendRequest<DeleteUser>(`/user/delete/${userId}`, "DELETE")
}

export const getMantraps = async (data: GetMantrapsRequest) => {
  return sendRequest<GetMantraps>(`/mantrap/list`, "POST", data)
}

export const addMantrap = async (data: AddMantrapRequest) => {
  return sendRequest<AddMantrap>(`/mantrap/add`, "POST", data)
}

export const updateMantrap = async (mantrapId: number, data: UpdateMantrapRequest) => {
  return sendRequest<UpdateMantrap>(`/mantrap/update/${mantrapId}`, "POST", data)
}


export const deleteMantrap = async (mantrapId: number) => {
  return sendRequest<DeleteMantrap>(`/mantrap/delete/${mantrapId}`, "DELETE")
}

export const commandDoor = async (mantrapId: number, action: string) => {
  return sendRequest<CommandDoor>(`/mantrap/${mantrapId}/${action}`)
}

export const enrollBiometrics = async (userId: number, formData: FormData) => {
  return sendRequest<EnrollBiometrics>(`/biometrics/enroll/${userId}`, "POST", {}, formData)
}

export const deleteBiometrics = async (userId: number) => {
  return sendRequest<DeleteBiometrics>(`/biometrics/delete/${userId}`, "DELETE")
}

export const getLogs = async (data: GetLogsRequest) => {
  return sendRequest<GetLogs>(`/log/list`, "POST", data)
}

export const getLogFile = async (filename: string) => {
  // Returns a file
}


