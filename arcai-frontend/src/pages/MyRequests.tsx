import React, { useState, useEffect } from 'react';
import apiService from '../services/apiService';
import { BorrowingRecordDto, PagingMetadata } from '../types';
import { parsePaginationHeader } from '../utils/parsePaginationHeader';
import { formatDateTime, formatDate } from '../utils/dateFormatter';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { Button } from '../components/ui/button';
import { Loader2, FileText } from 'lucide-react';
import { cn } from '../lib/utils';

const MyRequests: React.FC = () => {
  const [requests, setRequests] = useState<BorrowingRecordDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [paginationMetadata, setPaginationMetadata] = useState<PagingMetadata | null>(null);

  useEffect(() => {
    fetchRequests();
  }, [pageNumber]);

  const fetchRequests = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await apiService.get<BorrowingRecordDto[]>('/borrowing/my-requests', {
        params: { pageNumber, pageSize },
      });

      setRequests(response.data);
      
      const paginationHeader = response.headers['x-pagination'];
      if (paginationHeader) {
        const metadata = parsePaginationHeader(paginationHeader);
        setPaginationMetadata(metadata);
      }
    } catch (err: any) {
      setError('An error occurred while loading requests');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'Approved':
        return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'CheckedOut':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'Returned':
        return 'bg-gray-100 text-gray-800 border-gray-200';
      case 'Overdue':
        return 'bg-red-100 text-red-800 border-red-200';
      case 'Rejected':
        return 'bg-red-100 text-red-800 border-red-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'Pending':
        return 'Pending';
      case 'Approved':
        return 'Approved';
      case 'CheckedOut':
        return 'Checked Out';
      case 'Returned':
        return 'Returned';
      case 'Overdue':
        return 'Overdue';
      case 'Rejected':
        return 'Rejected';
      default:
        return status;
    }
  };


  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">My Requests</h1>
        <p className="mt-2 text-muted-foreground">
          View your document borrowing requests
        </p>
      </div>

      {error && (
        <div className="rounded-lg bg-destructive/10 p-4 text-sm text-destructive">
          {error}
        </div>
      )}

      {loading ? (
        <div className="flex items-center justify-center py-12">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
        </div>
      ) : (
        <>
          {requests.length === 0 ? (
            <Card>
              <CardContent className="py-12 text-center">
                <FileText className="mx-auto h-12 w-12 text-muted-foreground" />
                <p className="mt-4 text-lg font-semibold">No requests yet</p>
                <p className="mt-2 text-sm text-muted-foreground">
                  You can request documents from the Documents page
                </p>
              </CardContent>
            </Card>
          ) : (
            <Card>
              <CardHeader>
                <CardTitle>Request History</CardTitle>
              </CardHeader>
              <CardContent>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Document</TableHead>
                      <TableHead>Request Date</TableHead>
                      <TableHead>Due Date</TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead>Approval Date</TableHead>
                      <TableHead>Checkout Date</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {requests.map((request) => (
                      <TableRow key={request.id}>
                        <TableCell className="font-medium">
                          {request.document?.title || '-'}
                        </TableCell>
                        <TableCell>{formatDateTime(request.requestDate)}</TableCell>
                        <TableCell>{formatDate(request.dueDate)}</TableCell>
                        <TableCell>
                          <div className="flex flex-col gap-1">
                            <Badge className={cn('border', getStatusColor(request.status))}>
                              {getStatusText(request.status)}
                            </Badge>
                            {request.status === 'Approved' && (
                              <span className="text-xs text-blue-600 font-medium">
                                ✓ Ready for pickup
                              </span>
                            )}
                          </div>
                        </TableCell>
                        <TableCell>{formatDateTime(request.approvalDate)}</TableCell>
                        <TableCell>{formatDateTime(request.checkoutDate)}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>

                {paginationMetadata && paginationMetadata.totalPages > 1 && (
                  <div className="mt-4 flex items-center justify-center gap-2">
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
                )}
              </CardContent>
            </Card>
          )}
        </>
      )}
    </div>
  );
};

export default MyRequests;
