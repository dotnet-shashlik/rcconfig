// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/** 获取文件列表 GET /Logs/{resourceId} */
export async function logList(resourceId: string, params: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/Logs/${resourceId}`, {
    method: 'GET',
    params: params,
    ...(options || {})
  });
}

/** 获取文件详情 GET /Logs/{resourceId}/{logId} */
export async function logDetail(resourceId: string, logId: number, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/Logs/${resourceId}/${logId}`, {
    method: 'GET',
    ...(options || {})
  });
}