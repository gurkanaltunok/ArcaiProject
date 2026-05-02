// DTO Types
export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
}

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
}

export interface DocumentDto {
  id: number;
  title: string;
  status: string;
  description?: string;
  createdAt: string;
  documentType?: DocumentTypeDto;
  location?: LocationDto;
  course?: CourseDto;
  academicPeriod?: AcademicPeriodDto;
  addedByUser?: User;
  tags: TagDto[];
}

export interface DocumentTypeDto {
  id: number;
  name: string;
  description?: string;
}

export interface LocationDto {
  id: number;
  friendlyName: string;
  room: string;
  cabinet?: string;
  shelf?: string;
}

export interface CourseDto {
  id: number;
  courseCode: string;
  courseName: string;
  department?: string;
}

export interface AcademicPeriodDto {
  id: number;
  periodName: string;
  startDate?: string;
  endDate?: string;
}

export interface TagDto {
  id: number;
  name: string;
}

export interface CreateUserDto {
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  role: string;
}

export interface CreateDocumentDto {
  title: string;
  description?: string;
  documentTypeId: number;
  locationId: number;
  courseId?: number;
  academicPeriodId?: number;
  tagIds: number[];
}

export interface BorrowingRecordDto {
  id: number;
  status: string;
  requestDate: string;
  approvalDate?: string;
  checkoutDate?: string;
  dueDate?: string;
  returnDate?: string;
  document?: DocumentDto;
  requesterUser?: User;
  approverUser?: User;
}

export interface CreateBorrowingRequestDto {
  documentId: number;
  dueDate: string;
}

export interface NotificationDto {
  id: number;
  userId: number;
  message: string;
  link?: string;
  isRead: boolean;
  createdAt: string;
}

// Pagination Types
export interface PagingMetadata {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface PagingParameters {
  pageNumber: number;
  pageSize: number;
}

