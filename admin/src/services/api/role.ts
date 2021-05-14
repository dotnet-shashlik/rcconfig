// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/** 获取角色资源列表 GET /roles/{role}/resources */
export async function resourceList(role: string, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/roles/${role}/resources`, {
    method: 'GET',
    ...(options || {})
  });
}

/** 获取角色列表 GET /roles */
export async function roleList(options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/roles`, {
    method: 'GET',
    ...(options || {})
  });
}

/** 创建角色 POST /roles */
export async function createRole(body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/roles`, {
    method: 'POST',
    data: body,
    ...(options || {})
  });
}

/** 删除角色 DELETE /roles/{role} */
export async function deleteRole(role: string, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/roles/${role}`, {
    method: 'DELETE',
    ...(options || {})
  });
}