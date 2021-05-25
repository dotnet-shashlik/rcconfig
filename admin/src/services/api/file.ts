// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/** 获取文件列表 GET /ConfigurationFiles */
export async function fileList(resourceId: string, params: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/ConfigurationFiles/${resourceId}`, {
    method: 'GET',
    params: params,
    ...(options || {})
  });
}

/** 获取文件 POST /ConfigurationFiles/{resourceId} */
export async function createFile(resourceId: string, body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/ConfigurationFiles/${resourceId}`, {
    method: 'POST',
    data: body,
    ...(options || {})
  });
}

/** 更新文件 PATCH /ConfigurationFiles/{resourceId}/{fileId} */
export async function updateFile(resourceId: string, fileId: number, body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/ConfigurationFiles/${resourceId}/${fileId}`, {
    method: 'PATCH',
    data: body,
    ...(options || {})
  });
}

/** 删除文件 DELETE /ConfigurationFiles/{resourceId}/{fileId} */
export async function deleteFile(resourceId: string, fileId: number, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/ConfigurationFiles/${resourceId}/${fileId}`, {
    method: 'DELETE',
    ...(options || {})
  });
}