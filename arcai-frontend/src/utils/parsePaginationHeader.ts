import { PagingMetadata } from '../types';

export const parsePaginationHeader = (headerValue: string): PagingMetadata | null => {
  try {
    return JSON.parse(headerValue) as PagingMetadata;
  } catch (e) {
    return null;
  }
};

