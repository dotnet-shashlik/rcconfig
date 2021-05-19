// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/** 获取资源列表 GET /resources */
export async function resources(options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/resources`, {
    method: 'GET',
    ...(options || {})
  });
}

/** 绑定角色资源 POST /resources/{app}/{env}/bind */
export async function bindRoleResource(app: string, env: string, data: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/resources/${app}/${env}/bind`, {
    method: 'POST',
    data: data,
    ...(options || {})
  });
}

/** 解绑角色资源 DELETE /resources/{app}/{env}/bind */
export async function unbindRoleResource(app: string, env: string, data: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/resources/${app}/${env}/bind`, {
    method: 'DELETE',
    data: data,
    ...(options || {})
  });
}
