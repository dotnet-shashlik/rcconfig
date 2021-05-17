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

/** 绑定角色资源 GET /resources/{app}/{env}/bind */
export async function roleList(app: string, env: string, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/resources/${app}/${env}/bind`, {
    method: 'POST',
    ...(options || {})
  });
}
