// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/** 获取资源列表 GET /resources */
export async function resourceList(options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/resources`, {
    method: 'GET',
    ...(options || {})
  });
}

/** 获取已授权资源列表 GET /resources */
export async function resourceAuthList(searchModel: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/resources/authorizations`, {
    method: 'GET',
    params: searchModel,
    ...(options || {})
  });
}

/** 授权角色资源 POST /resources/{app}/{env}/bind */
export async function authRoleResource(data: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/resources/auth`, {
    method: 'POST',
    data,
    ...(options || {})
  });
}

/** 删除授权角色资源 DELETE /resources/{app}/{env}/bind */
export async function unAuthRoleResource(data: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/resources/auth`, {
    method: 'DELETE',
    data,
    ...(options || {})
  });
}