// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/** 获取环境列表 GET /environments */
export async function envList(options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/environments`, {
    method: 'GET',
    ...(options || {})
  });
}

/** 获取环境 POST /environments */
export async function createEnv(app: string, body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/environments/${app}`, {
    method: 'POST',
    data: body,
    ...(options || {})
  });
}

/** 更新环境 PATCH /environments */
export async function updateEnv(app: string, env: string, body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/environments/${app}/${env}`, {
    method: 'PATCH',
    data: body,
    ...(options || {})
  });
}

/** 删除环境 DELETE /environments */
export async function deleteEnv(app: string, env: string, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/environments/${app}/${env}`, {
    method: 'DELETE',
    ...(options || {})
  });
}