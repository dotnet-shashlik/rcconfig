// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/** 获取文件列表 GET /Files/{resourceId} */
export async function fileList(resourceId: string, params: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/Files/${resourceId}`, {
    method: 'GET',
    params: params,
    ...(options || {})
  });
}

/** 获取文件详情 GET /Files/{resourceId}/{fileId} */
export async function fileDetail(resourceId: string, fileId: number, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/Files/${resourceId}/${fileId}`, {
    method: 'GET',
    ...(options || {})
  });
}

/** 获取文件 POST /Files/{resourceId} */
export async function createFile(resourceId: string, body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/Files/${resourceId}`, {
    method: 'POST',
    data: body,
    ...(options || {})
  });
}

/** 更新文件 PATCH /Files/{resourceId}/{fileId} */
export async function updateFile(resourceId: string, fileId: number, body: any, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/Files/${resourceId}/${fileId}`, {
    method: 'PATCH',
    data: body,
    ...(options || {})
  });
}

/** 删除文件 DELETE /Files/{resourceId}/{fileId} */
export async function deleteFile(resourceId: string, fileId: number, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/Files/${resourceId}/${fileId}`, {
    method: 'DELETE',
    ...(options || {})
  });
}