import React, { useState, useEffect } from 'react';
import apiService from '../../services/apiService';
import { BorrowingRecordDto, PagingMetadata } from '../../types';
import { parsePaginationHeader } from '../../utils/parsePaginationHeader';
import { formatDateTime, formatDate } from '../../utils/dateFormatter';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { Badge } from '../../components/ui/badge';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../../components/ui/tabs';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../../components/ui/table';
import { CheckCircle, XCircle, ArrowLeft, Loader2, FileText } from 'lucide-react';
import { cn } from '../../lib/utils';

const AdminRequests: React.FC = () => {
  const [tabValue, setTabValue] = useState('pending');
  const [pendingRequests, setPendingRequests] = useState<BorrowingRecordDto[]>([]);
  const [approvedRequests, setApprovedRequests] = useState<BorrowingRecordDto[]>([]);
  const [borrowedDocuments, setBorrowedDocuments] = useState<BorrowingRecordDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [paginationMetadata, setPaginationMetadata] = useState<PagingMetadata | null>(null);
  const [processingId, setProcessingId] = useState<number | null>(null);

  useEffect(() => {
    if (tabValue === 'pending') {
      fetchPendingRequests();
    } else if (tabValue === 'approved') {
      fetchApprovedRequests();
    } else {
      fetchBorrowedDocuments();
    }
  }, [tabValue, pageNumber]);

  const fetchPendingRequests = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await apiService.get<BorrowingRecordDto[]>('/borrowing/pending', {
        params: { pageNumber, pageSize },
      });

      setPendingRequests(response.data);
      
      const paginationHeader = response.headers['x-pagination'];
      if (paginationHeader) {
        const metadata = parsePaginationHeader(paginationHeader);
        setPaginationMetadata(metadata);
      }
    } catch (err: any) {
      setError('An error occurred while loading pending requests');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchApprovedRequests = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await apiService.get<BorrowingRecordDto[]>('/borrowing/approved', {
        params: { pageNumber, pageSize },
      });

      setApprovedRequests(response.data);
      
      const paginationHeader = response.headers['x-pagination'];
      if (paginationHeader) {
        const metadata = parsePaginationHeader(paginationHeader);
        setPaginationMetadata(metadata);
      }
    } catch (err: any) {
      setError('An error occurred while loading approved requests');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchBorrowedDocuments = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await apiService.get<BorrowingRecordDto[]>('/borrowing/borrowed', {
        params: { pageNumber, pageSize },
      });

      setBorrowedDocuments(response.data);
      
      const paginationHeader = response.headers['x-pagination'];
      if (paginationHeader) {
        const metadata = parsePaginationHeader(paginationHeader);
        setPaginationMetadata(metadata);
      }
    } catch (err: any) {
      setError('An error occurred while loading borrowed documents');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleApprove = async (id: number) => {
    setProcessingId(id);
    try {
      await apiService.post(`/borrowing/${id}/approve`);
      alert('Request approved!');
      if (tabValue === 'pending') {
        fetchPendingRequests();
      }
      // Also refresh approved tab if it exists
      if (approvedRequests.length > 0 || tabValue === 'approved') {
        fetchApprovedRequests();
      }
    } catch (err: any) {
      alert(err.response?.data?.message || 'An error occurred while approving the request');
    } finally {
      setProcessingId(null);
    }
  };

  const handleReject = async (id: number) => {
    if (!window.confirm('Are you sure you want to reject this request?')) {
      return;
    }

    setProcessingId(id);
    try {
      await apiService.post(`/borrowing/${id}/reject`);
      alert('Request rejected!');
      if (tabValue === 'pending') {
        fetchPendingRequests();
      }
    } catch (err: any) {
      alert(err.response?.data?.message || 'An error occurred while rejecting the request');
    } finally {
      setProcessingId(null);
    }
  };

  const handleReturn = async (id: number) => {
    setProcessingId(id);
    try {
      await apiService.post(`/borrowing/${id}/return`);
      alert('Document returned!');
      if (tabValue === 'borrowed') {
        fetchBorrowedDocuments();
      }
    } catch (err: any) {
      alert(err.response?.data?.message || 'An error occurred while returning the document');
    } finally {
      setProcessingId(null);
    }
  };

  const handleCheckout = async (id: number) => {
    setProcessingId(id);
    try {
      await apiService.post(`/borrowing/${id}/checkout`);
      alert('Document checked out!');
      if (tabValue === 'pending') {
        fetchPendingRequests();
      } else if (tabValue === 'approved') {
        fetchApprovedRequests();
      } else if (tabValue === 'borrowed') {
        fetchBorrowedDocuments();
      }
    } catch (err: any) {
      alert(err.response?.data?.message || 'An error occurred while checking out the document');
    } finally {
      setProcessingId(null);
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
      case 'Overdue':
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
      case 'Overdue':
        return 'Overdue';
      default:
        return status;
    }
  };


  const getCurrentData = () => {
    if (tabValue === 'pending') return pendingRequests;
    if (tabValue === 'approved') return approvedRequests;
    return borrowedDocuments;
  };

  const currentData = getCurrentData();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Request Management</h1>
        <p className="mt-2 text-muted-foreground">
          Approve, reject document requests and manage return processes
        </p>
      </div>

      <Tabs value={tabValue} onValueChange={(value) => { setTabValue(value); setPageNumber(1); }}>
        <TabsList>
          <TabsTrigger value="pending">Pending Requests</TabsTrigger>
          <TabsTrigger value="approved">Approved Requests</TabsTrigger>
          <TabsTrigger value="borrowed">Borrowed Documents</TabsTrigger>
        </TabsList>

        <TabsContent value="pending" className="space-y-4">
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
            <Card>
              <CardHeader>
                <CardTitle>Pending Requests</CardTitle>
              </CardHeader>
              <CardContent>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Document</TableHead>
                      <TableHead>Requester</TableHead>
                      <TableHead>Request Date</TableHead>
                      <TableHead>Due Date</TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead className="text-right">Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {currentData.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={6} className="text-center py-8">
                          <FileText className="mx-auto h-12 w-12 text-muted-foreground mb-4" />
                          <p className="text-muted-foreground">No pending requests</p>
                        </TableCell>
                      </TableRow>
                    ) : (
                      currentData.map((request) => (
                        <TableRow key={request.id}>
                          <TableCell className="font-medium">
                            {request.document?.title || '-'}
                          </TableCell>
                          <TableCell>
                            {request.requesterUser?.firstName} {request.requesterUser?.lastName}
                          </TableCell>
                          <TableCell>{formatDateTime(request.requestDate)}</TableCell>
                          <TableCell>{formatDate(request.dueDate)}</TableCell>
                          <TableCell>
                            <Badge className={cn('border', getStatusColor(request.status))}>
                              {getStatusText(request.status)}
                            </Badge>
                          </TableCell>
                          <TableCell className="text-right">
                            <div className="flex items-center justify-end gap-2">
                              <Button
                                variant="outline"
                                size="sm"
                                onClick={() => handleApprove(request.id)}
                                disabled={processingId === request.id}
                              >
                                <CheckCircle className="mr-2 h-4 w-4" />
                                Approve
                              </Button>
                              <Button
                                variant="outline"
                                size="sm"
                                onClick={() => handleReject(request.id)}
                                disabled={processingId === request.id}
                                className="text-destructive hover:text-destructive"
                              >
                                <XCircle className="mr-2 h-4 w-4" />
                                Reject
                              </Button>
                            </div>
                          </TableCell>
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          )}
        </TabsContent>

        <TabsContent value="approved" className="space-y-4">
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
            <Card>
              <CardHeader>
                <CardTitle>Approved Requests - Ready for Pickup</CardTitle>
              </CardHeader>
              <CardContent>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Document</TableHead>
                      <TableHead>Requester</TableHead>
                      <TableHead>Request Date</TableHead>
                      <TableHead>Approval Date</TableHead>
                      <TableHead>Due Date</TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead className="text-right">Action</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {currentData.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={7} className="text-center py-8">
                          <FileText className="mx-auto h-12 w-12 text-muted-foreground mb-4" />
                          <p className="text-muted-foreground">No approved requests</p>
                        </TableCell>
                      </TableRow>
                    ) : (
                      currentData.map((request) => (
                        <TableRow key={request.id}>
                          <TableCell className="font-medium">
                            {request.document?.title || '-'}
                          </TableCell>
                          <TableCell>
                            {request.requesterUser?.firstName} {request.requesterUser?.lastName}
                          </TableCell>
                          <TableCell>{formatDateTime(request.requestDate)}</TableCell>
                          <TableCell>{formatDateTime(request.approvalDate)}</TableCell>
                          <TableCell>{formatDate(request.dueDate)}</TableCell>
                          <TableCell>
                            <Badge className={cn('border', getStatusColor(request.status))}>
                              {getStatusText(request.status)}
                            </Badge>
                          </TableCell>
                          <TableCell className="text-right">
                            <Button
                              variant="default"
                              size="sm"
                              onClick={() => handleCheckout(request.id)}
                              disabled={processingId === request.id}
                            >
                              Checked Out
                            </Button>
                          </TableCell>
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          )}
        </TabsContent>

        <TabsContent value="borrowed" className="space-y-4">
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
            <Card>
              <CardHeader>
                <CardTitle>Borrowed Documents</CardTitle>
              </CardHeader>
              <CardContent>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Document</TableHead>
                      <TableHead>Requester</TableHead>
                      <TableHead>Request Date</TableHead>
                      <TableHead>Due Date</TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead className="text-right">Action</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {currentData.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={6} className="text-center py-8">
                          <FileText className="mx-auto h-12 w-12 text-muted-foreground mb-4" />
                          <p className="text-muted-foreground">No borrowed documents</p>
                        </TableCell>
                      </TableRow>
                    ) : (
                      currentData.map((request) => (
                        <TableRow key={request.id}>
                          <TableCell className="font-medium">
                            {request.document?.title || '-'}
                          </TableCell>
                          <TableCell>
                            {request.requesterUser?.firstName} {request.requesterUser?.lastName}
                          </TableCell>
                          <TableCell>{formatDateTime(request.requestDate)}</TableCell>
                          <TableCell>{formatDate(request.dueDate)}</TableCell>
                          <TableCell>
                            <Badge className={cn('border', getStatusColor(request.status))}>
                              {getStatusText(request.status)}
                            </Badge>
                          </TableCell>
                          <TableCell className="text-right">
                            <Button
                              variant="outline"
                              size="sm"
                              onClick={() => handleReturn(request.id)}
                              disabled={processingId === request.id}
                            >
                              <ArrowLeft className="mr-2 h-4 w-4" />
                              Return
                            </Button>
                          </TableCell>
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          )}
        </TabsContent>
      </Tabs>

      {paginationMetadata && paginationMetadata.totalPages > 1 && (
        <div className="flex items-center justify-center gap-2">
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
    </div>
  );
};

export default AdminRequests;
