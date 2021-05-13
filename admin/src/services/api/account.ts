// @ts-ignore
/* eslint-disable */
import { request } from 'umi';
import { message } from 'antd';

/** 获取当前的用户 GET /users/current */
export async function currentUser(options?: { [key: string]: any }) {
  let res = await request<API.Response<API.CurrentUser>>('/users/current', {
    method: 'GET',
    ...(options || {}),
  });

  if (res.success)
    res.data!.avatar = 'https://img.youxi369.com/article/contents/2021/02/10/small_20210210112721737.jpeg';
  else
    message.error(`用户信息加载失败: ${res.msg}`);
  return res.data;
}

/** 登录接口 POST /connect/token */
export async function login(body: API.LoginParams, options?: { [key: string]: any }) {
  return request<API.LoginResult>('/connect/token', {
    method: 'POST',
    requestType: 'form',
    data: body,
    ...(options || {}),
  });
}

/** 修改密码 PATCH /users/password */
export async function changePassword(body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>('/users/password', {
    method: 'PATCH',
    data: body,
    ...(options || {})
  });
}

/** 创建用户 POST /users */
export async function createUser(body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>('/users', {
    method: 'POST',
    data: body,
    ...(options || {})
  });
}

/** 创建用户 POST /users */
export async function userList(body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>('/users', {
    method: 'POST',
    data: body,
    ...(options || {})
  });
}

/** 删除用户 DELETE /users */
export async function deleteUser(userId: number, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/users/${userId}`, {
    method: 'DELETE',
    ...(options || {})
  });
}

/** 获取用户资料列表 GET /users/{userId}/resources */
export async function resourceList(userId: number, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/users/${userId}/resources`, {
    method: 'GET',
    ...(options || {})
  });
}