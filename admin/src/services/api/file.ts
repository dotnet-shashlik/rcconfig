// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/** 获取环境列表 GET /ConfigurationFiles */
export async function envList(params: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/ConfigurationFiles`, {
    method: 'GET',
    params: params,
    ...(options || {})
  });
}

/** 获取环境 POST /ConfigurationFiles */
export async function createEnv(app: string, env: string, body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/ConfigurationFiles/${app}/${env}`, {
    method: 'POST',
    data: body,
    ...(options || {})
  });
}

/** 更新环境 PATCH /ConfigurationFiles */
export async function updateEnv(app: string, env: string, fileId: number, body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/ConfigurationFiles/${app}/${env}/${fileId}`, {
    method: 'PATCH',
    data: body,
    ...(options || {})
  });
}

/** 删除环境 DELETE /ConfigurationFiles */
export async function deleteEnv(app: string, env: string, fileId: number, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/ConfigurationFiles/${app}/${env}/${fileId}`, {
    method: 'DELETE',
    ...(options || {})
  });
}