import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import apiService from '../services/apiService';
import { DocumentDto, PagingMetadata, CreateBorrowingRequestDto, TagDto } from '../types';
import { parsePaginationHeader } from '../utils/parsePaginationHeader';
import { getTodayInTRNC } from '../utils/dateFormatter';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';
import { Input } from '../components/ui/input';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '../components/ui/dialog';
import { Label } from '../components/ui/label';
import { Loader2, Search, Plus, BookOpen, MapPin, Tag, FileText } from 'lucide-react';
import { cn } from '../lib/utils';

const Home: React.FC = () => {
  const { role } = useAuth();
  const [documents, setDocuments] = useState<DocumentDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(12);
  const [paginationMetadata, setPaginationMetadata] = useState<PagingMetadata | null>(null);
  const [requestDialogOpen, setRequestDialogOpen] = useState(false);
  const [selectedDocument, setSelectedDocument] = useState<DocumentDto | null>(null);
  const [dueDate, setDueDate] = useState('');
  const [requesting, setRequesting] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');

  useEffect(() => {
    fetchDocuments();
  }, [pageNumber]);

  const fetchDocuments = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await apiService.get<DocumentDto[]>('/documents', {
        params: { pageNumber, pageSize },
      });

      setDocuments(response.data);
      
      const paginationHeader = response.headers['x-pagination'];
      if (paginationHeader) {
        const metadata = parsePaginationHeader(paginationHeader);
        setPaginationMetadata(metadata);
      }
    } catch (err: any) {
      setError('An error occurred while loading documents');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleRequestClick = (document: DocumentDto) => {
    if (document.status !== 'Available') {
      const message = document.status === 'CheckedOut' 
        ? 'This document is currently checked out and cannot be requested until it is returned.'
        : document.status === 'Missing'
        ? 'This document is marked as missing and cannot be requested.'
        : 'This document is not available for borrowing.';
      alert(message);
      return;
    }
    setSelectedDocument(document);
    setDueDate('');
    setRequestDialogOpen(true);
  };

  const handleRequestSubmit = async () => {
    if (!selectedDocument || !dueDate) {
      return;
    }

    setRequesting(true);
    try {
      const requestDto: CreateBorrowingRequestDto = {
        documentId: selectedDocument.id,
        dueDate: new Date(dueDate).toISOString(),
      };

      await apiService.post('/borrowing/request', requestDto);
      alert('Request created successfully!');
      setRequestDialogOpen(false);
      fetchDocuments();
    } catch (err: any) {
      alert(err.response?.data?.message || 'An error occurred while creating the request');
    } finally {
      setRequesting(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Available':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'CheckedOut':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'Missing':
        return 'bg-red-100 text-red-800 border-red-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'Available':
        return 'Available';
      case 'CheckedOut':
        return 'Checked Out';
      case 'Missing':
        return 'Missing';
      default:
        return status;
    }
  };

  const filteredDocuments = documents.filter((doc) =>
    doc.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
    doc.documentType?.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    doc.course?.courseName.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Documents</h1>
        {role === 'Professor' && (
          <p className="mt-2 text-muted-foreground">
            View and request available documents
          </p>
        )}
      </div>

      {/* Search Bar */}
      <div className="relative">
        <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          type="text"
          placeholder="Search documents, types, or courses..."
          className="pl-10"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
        />
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
          {filteredDocuments.length === 0 ? (
            <Card>
              <CardContent className="py-12 text-center">
                <BookOpen className="mx-auto h-12 w-12 text-muted-foreground" />
                <p className="mt-4 text-lg font-semibold">No documents found</p>
                <p className="mt-2 text-sm text-muted-foreground">
                  {searchQuery ? 'No documents match your search criteria' : 'No documents have been added yet'}
                </p>
              </CardContent>
            </Card>
          ) : (
            <>
              {/* Document Grid */}
              <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                {filteredDocuments.map((doc) => (
                  <Card key={doc.id} className="flex flex-col hover:shadow-lg transition-shadow">
                    <CardHeader>
                      <div className="flex items-start justify-between">
                        <CardTitle className="text-lg leading-tight">{doc.title}</CardTitle>
                        <Badge className={cn('ml-2', getStatusColor(doc.status))}>
                          {getStatusText(doc.status)}
                        </Badge>
                      </div>
                      {doc.description && (
                        <CardDescription className="line-clamp-2">
                          {doc.description}
                        </CardDescription>
                      )}
                    </CardHeader>
                    <CardContent className="flex-1 space-y-3">
                      {doc.documentType && (
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <FileText className="h-4 w-4" />
                          <span>{doc.documentType.name}</span>
                        </div>
                      )}
                      {doc.location && (
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <MapPin className="h-4 w-4" />
                          <span>{doc.location.friendlyName}</span>
                        </div>
                      )}
                      {doc.course && (
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <BookOpen className="h-4 w-4" />
                          <span>{doc.course.courseName}</span>
                        </div>
                      )}
                      {doc.tags && doc.tags.length > 0 && (
                        <div className="flex flex-wrap gap-2">
                          {doc.tags.map((tag: TagDto) => (
                            <Badge key={tag.id} variant="outline" className="text-xs">
                              <Tag className="mr-1 h-3 w-3" />
                              {tag.name}
                            </Badge>
                          ))}
                        </div>
                      )}
                    </CardContent>
                    {role === 'Professor' && (
                      <CardFooter>
                        <Button
                          onClick={() => handleRequestClick(doc)}
                          disabled={doc.status !== 'Available'}
                          className="w-full"
                          variant={doc.status === 'Available' ? 'default' : 'outline'}
                        >
                          <Plus className="mr-2 h-4 w-4" />
                          {doc.status === 'Available' ? 'Request' : 'Not Available'}
                        </Button>
                      </CardFooter>
                    )}
                  </Card>
                ))}
              </div>

              {/* Pagination */}
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
            </>
          )}
        </>
      )}

      {/* Request Dialog */}
      <Dialog open={requestDialogOpen} onOpenChange={setRequestDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Request Document</DialogTitle>
            <DialogDescription>
              You are requesting <strong>{selectedDocument?.title}</strong>.
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="dueDate">Return Date</Label>
              <Input
                id="dueDate"
                type="date"
                value={dueDate}
                onChange={(e) => setDueDate(e.target.value)}
                required
                min={getTodayInTRNC()}
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setRequestDialogOpen(false)}>
              Cancel
            </Button>
            <Button
              onClick={handleRequestSubmit}
              disabled={!dueDate || requesting}
            >
              {requesting ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Processing...
                </>
              ) : (
                'Request'
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default Home;
