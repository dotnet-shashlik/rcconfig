// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/** 获取角色资源列表 GET /applications */
export async function appList(options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/applications`, {
    method: 'GET',
    ...(options || {})
  });
}

/** 创建角色 POST /applications */
export async function createApp(body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/applications`, {
    method: 'POST',
    data: body,
    ...(options || {})
  });
}

/** 更新应用 PATCH /applications */
export async function updateApp(app: string, body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/applications/${app}`, {
    method: 'PATCH',
    data: body,
    ...(options || {})
  });
}

/** 删除应用 DELETE /applications */
export async function deleteApp(app: string, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/applications/${app}`, {
    method: 'DELETE',
    ...(options || {})
  });
}