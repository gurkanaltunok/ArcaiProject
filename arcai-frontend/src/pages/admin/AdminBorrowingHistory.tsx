import React, { useState, useEffect } from 'react';
import apiService from '../../services/apiService';
import { BorrowingRecordDto, PagingMetadata, User, DocumentTypeDto } from '../../types';
import { parsePaginationHeader } from '../../utils/parsePaginationHeader';
import { formatDateTime, formatDate } from '../../utils/dateFormatter';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { Badge } from '../../components/ui/badge';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../../components/ui/select';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../../components/ui/table';
import { History, Loader2, X, Filter } from 'lucide-react';
import { cn } from '../../lib/utils';

const AdminBorrowingHistory: React.FC = () => {
  const [records, setRecords] = useState<BorrowingRecordDto[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [documentTypes, setDocumentTypes] = useState<DocumentTypeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(20);
  const [paginationMetadata, setPaginationMetadata] = useState<PagingMetadata | null>(null);

  // Filter states
  const [filterUserId, setFilterUserId] = useState<number | undefined>(undefined);
  const [filterDocumentTypeId, setFilterDocumentTypeId] = useState<number | undefined>(undefined);
  const [filterStatus, setFilterStatus] = useState<string | undefined>(undefined);
  const [dateFrom, setDateFrom] = useState<string>('');
  const [dateTo, setDateTo] = useState<string>('');

  useEffect(() => {
    fetchLookupData();
  }, []);

  useEffect(() => {
    fetchRecords();
  }, [pageNumber, filterUserId, filterDocumentTypeId, filterStatus, dateFrom, dateTo]);

  const fetchLookupData = async () => {
    try {
      const [usersRes, docTypesRes] = await Promise.all([
        apiService.get<User[]>('/users', { params: { pageNumber: 1, pageSize: 1000 } }),
        apiService.get<DocumentTypeDto[]>('/documenttypes', { params: { pageNumber: 1, pageSize: 100 } }),
      ]);

      setUsers(usersRes.data);
      setDocumentTypes(docTypesRes.data);
    } catch (err) {
      console.error('Error loading lookup data:', err);
    }
  };

  const fetchRecords = async () => {
    setLoading(true);
    setError('');
    try {
      const params: any = {
        pageNumber,
        pageSize,
      };

      if (filterUserId && filterUserId > 0) {
        params.RequesterUserId = filterUserId;
      }
      if (filterDocumentTypeId && filterDocumentTypeId > 0) {
        params.DocumentTypeIdForBorrowing = filterDocumentTypeId;
      }
      if (filterStatus && filterStatus !== '') {
        params.BorrowingStatus = filterStatus;
      }
      if (dateFrom) {
        params.DateFrom = new Date(dateFrom).toISOString();
      }
      if (dateTo) {
        params.DateTo = new Date(dateTo).toISOString();
      }

      const response = await apiService.get<BorrowingRecordDto[]>('/borrowing/all', {
        params,
      });

      setRecords(response.data);

      const paginationHeader = response.headers['x-pagination'];
      if (paginationHeader) {
        const metadata = parsePaginationHeader(paginationHeader);
        setPaginationMetadata(metadata);
      }
    } catch (err: any) {
      setError('An error occurred while loading borrowing records');
      console.error('Error loading borrowing records:', err);
      console.error('Error response:', err.response?.data);
      console.error('Error status:', err.response?.status);
    } finally {
      setLoading(false);
    }
  };

  const handleFilterChange = () => {
    setPageNumber(1);
  };

  const handleClearFilters = () => {
    setFilterUserId(undefined);
    setFilterDocumentTypeId(undefined);
    setFilterStatus(undefined);
    setDateFrom('');
    setDateTo('');
    setPageNumber(1);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'Approved':
        return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'Rejected':
        return 'bg-red-100 text-red-800 border-red-200';
      case 'CheckedOut':
        return 'bg-[#122749]/10 text-[#122749] border-[#122749]/20';
      case 'Returned':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'Overdue':
        return 'bg-orange-100 text-orange-800 border-orange-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };


  const hasActiveFilters = filterUserId || filterDocumentTypeId || filterStatus || dateFrom || dateTo;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Borrowing History</h1>
          <p className="mt-2 text-muted-foreground">
            View all borrowing records with detailed information
          </p>
        </div>
      </div>

      {error && (
        <div className="rounded-lg bg-destructive/10 p-4 text-sm text-destructive">
          {error}
        </div>
      )}

      {/* Filters */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <Filter className="h-5 w-5" />
              Filters
            </CardTitle>
            {hasActiveFilters && (
              <Button variant="ghost" size="sm" onClick={handleClearFilters}>
                <X className="mr-2 h-4 w-4" />
                Clear Filters
              </Button>
            )}
          </div>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
            <div className="space-y-2">
              <Label htmlFor="userFilter">User</Label>
              <Select
                value={filterUserId?.toString() || ''}
                onValueChange={(value) => {
                  setFilterUserId(value === '__clear__' ? undefined : parseInt(value));
                  handleFilterChange();
                }}
              >
                <SelectTrigger id="userFilter">
                  <SelectValue placeholder="All Users" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="__clear__">All Users</SelectItem>
                  {users.map((user) => (
                    <SelectItem key={user.id} value={user.id.toString()}>
                      {user.firstName} {user.lastName} ({user.email})
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="documentTypeFilter">Document Type</Label>
              <Select
                value={filterDocumentTypeId?.toString() || ''}
                onValueChange={(value) => {
                  setFilterDocumentTypeId(value === '__clear__' ? undefined : parseInt(value));
                  handleFilterChange();
                }}
              >
                <SelectTrigger id="documentTypeFilter">
                  <SelectValue placeholder="All Types" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="__clear__">All Types</SelectItem>
                  {documentTypes.map((type) => (
                    <SelectItem key={type.id} value={type.id.toString()}>
                      {type.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="statusFilter">Status</Label>
              <Select
                value={filterStatus || ''}
                onValueChange={(value) => {
                  setFilterStatus(value === '__clear__' ? undefined : value);
                  handleFilterChange();
                }}
              >
                <SelectTrigger id="statusFilter">
                  <SelectValue placeholder="All Statuses" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="__clear__">All Statuses</SelectItem>
                  <SelectItem value="Pending">Pending</SelectItem>
                  <SelectItem value="Approved">Approved</SelectItem>
                  <SelectItem value="Rejected">Rejected</SelectItem>
                  <SelectItem value="CheckedOut">Checked Out</SelectItem>
                  <SelectItem value="Returned">Returned</SelectItem>
                  <SelectItem value="Overdue">Overdue</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="dateFrom">From Date</Label>
              <Input
                id="dateFrom"
                type="date"
                value={dateFrom}
                onChange={(e) => {
                  setDateFrom(e.target.value);
                  handleFilterChange();
                }}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="dateTo">To Date</Label>
              <Input
                id="dateTo"
                type="date"
                value={dateTo}
                onChange={(e) => {
                  setDateTo(e.target.value);
                  handleFilterChange();
                }}
              />
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Records Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <History className="h-5 w-5" />
            All Borrowing Records
            {paginationMetadata && (
              <span className="text-sm font-normal text-muted-foreground ml-2">
                ({paginationMetadata.totalCount} total)
              </span>
            )}
          </CardTitle>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="flex items-center justify-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
          ) : records.length === 0 ? (
            <div className="text-center py-12 text-muted-foreground">
              No borrowing records found.
            </div>
          ) : (
            <>
              <div className="rounded-md border">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>ID</TableHead>
                      <TableHead>Document</TableHead>
                      <TableHead>Requester</TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead>Request Date</TableHead>
                      <TableHead>Approval Date</TableHead>
                      <TableHead>Checkout Date</TableHead>
                      <TableHead>Due Date</TableHead>
                      <TableHead>Return Date</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {records.map((record) => (
                      <TableRow key={record.id}>
                        <TableCell className="font-medium">{record.id}</TableCell>
                        <TableCell>
                          <div>
                            <div className="font-medium">{record.document?.title || 'N/A'}</div>
                            {record.document?.documentType && (
                              <div className="text-sm text-muted-foreground">
                                {record.document.documentType.name}
                              </div>
                            )}
                          </div>
                        </TableCell>
                        <TableCell>
                          {record.requesterUser
                            ? `${record.requesterUser.firstName} ${record.requesterUser.lastName}`
                            : 'N/A'}
                        </TableCell>
                        <TableCell>
                          <Badge className={cn('border', getStatusColor(record.status))}>
                            {record.status}
                          </Badge>
                        </TableCell>
                        <TableCell>{formatDateTime(record.requestDate)}</TableCell>
                        <TableCell>{formatDateTime(record.approvalDate)}</TableCell>
                        <TableCell>{formatDateTime(record.checkoutDate)}</TableCell>
                        <TableCell>{formatDate(record.dueDate)}</TableCell>
                        <TableCell>{formatDateTime(record.returnDate)}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>

              {/* Pagination */}
              {paginationMetadata && paginationMetadata.totalPages > 1 && (
                <div className="flex items-center justify-between mt-4">
                  <div className="text-sm text-muted-foreground">
                    Showing {((pageNumber - 1) * pageSize) + 1} to{' '}
                    {Math.min(pageNumber * pageSize, paginationMetadata.totalCount)} of{' '}
                    {paginationMetadata.totalCount} records
                  </div>
                  <div className="flex items-center gap-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => setPageNumber((p) => Math.max(1, p - 1))}
                      disabled={pageNumber === 1}
                    >
                      Previous
                    </Button>
                    <span className="text-sm text-muted-foreground">
                      Page {pageNumber} / {paginationMetadata.totalPages}
                    </span>
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => setPageNumber((p) => Math.min(paginationMetadata.totalPages, p + 1))}
                      disabled={pageNumber === paginationMetadata.totalPages}
                    >
                      Next
                    </Button>
                  </div>
                </div>
              )}
            </>
          )}
        </CardContent>
      </Card>
    </div>
  );
};

export default AdminBorrowingHistory;

