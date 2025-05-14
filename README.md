# GHLearning-EasySerilog
[![GitHub Actions GHLearning-EasySerilog](https://github.com/gordon-hung/GHLearning-EasySerilog/actions/workflows/dotnet.yml/badge.svg)](https://github.com/gordon-hung/GHLearning-EasySerilog/actions/workflows/dotnet.yml) [![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/gordon-hung/GHLearning-EasySerilog)

## Purpose and Scope
GHLearning-EasySerilog is a demonstration repository that showcases the integration of Serilog - a powerful structured logging library for .NET - with various logging outputs and monitoring systems. The repository provides several sample applications, each demonstrating a different integration pattern with Serilog. This document provides a high-level overview of the system architecture, components, and the specific use cases demonstrated by each sample application.

For detailed information about the architectural patterns used, see Architecture. For specific information about each sample application, see Sample Applications.

## Core Concepts
The repository demonstrates several key concepts related to structured logging with Serilog:

1. Structured Logging - Using Serilog to generate logs with semantic information rather than plain text
2. Multiple Sink Destinations - Directing log data to various outputs (console, file, specialized systems)
3. Correlation ID Tracking - Maintaining request context across service boundaries
4. Log Enrichment - Adding contextual information to log entries
5. Integration with Monitoring Tools - Connecting logs with specialized monitoring and alerting platforms

## Benefits and Usage
The Correlation ID System provides several key benefits:

1. Request Tracing: Enables tracing of requests across distributed systems
2. Log Correlation: Allows logs from different services to be correlated by ID
3. Debugging: Simplifies troubleshooting by providing a unique identifier for each request chain
4. Distributed Tracing: Integrates with the .NET Activity API for improved observability

When a client includes a correlation ID in their request, the system will preserve and propagate that ID. Otherwise, a new ID is generated automatically, ensuring that every request is uniquely identifiable throughout its lifecycle.