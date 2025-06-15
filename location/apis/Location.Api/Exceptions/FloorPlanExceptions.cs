namespace Location.Api.Exceptions;

public class BadRequestException(string message) : Exception(message);

public class FloorPlanAlreadyExistsForLevel(int floorLevel) 
    : Exception($"Floor plan already exists for floor level {floorLevel}");

public class FileSizeExceedsLimit() 
    : Exception("File size exceeds 2MB limit");

public class ResourceAndFloorPlanLocationMismatch() 
    : Exception("Resource and floor plan must belong to the same location");