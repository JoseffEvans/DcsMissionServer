---@class Request
---@field Points table<integer, table<integer, number>>

---@param message string
---@param time number?
function OutText(message, time)
    time = time or 100
    trigger.action.outText(message, time)
end

function AppendSocketPath()
    package.path = package.path .. ";.\\LuaSocket\\?.lua"
    package.cpath = package.cpath .. ";.\\LuaSocket\\?.dll"
end

function SendTest()
    local allUnits = {
        Units = {}
    }
    for i = 1, 2 do
        for _, group in ipairs(coalition.getGroups(i)) do
            for _, unit in ipairs(group:getUnits()) do
                local unitPos = unit:getPoint()
                allUnits.Units[#allUnits.Units+1] = {
                    X = unitPos.x,
                    Y = unitPos.z,
                    Team = i - 1
                }
            end
        end
    end
    AppendSocketPath()
    local socket = require("socket")
    local con, err = socket.connect("localhost", 12622)
    if err then trigger.action.outText(net.lua2json(err), 100) end
    con:send(net.lua2json(allUnits))
    con:close()
end


function CreateServer()

    local tcpLib = require("socket")
    local server = tcpLib.bind("127.0.0.1", 12522)
    if not server then return end

    server:settimeout(0)
    return server
end

NPoints = 400
ThingN = 5902

function RunServerLoop()
    local server = CreateServer()

    local function PrintAccept()
        if not server then
            OutText("Server was nil when running loop")
            return
        end

        local socket = server:accept();

        if socket then
            local message, err = socket:receive("*a")
            if err then OutText("Error occurred: " .. err) end
            -- if message then OutText("Message Recived: " .. message) end

            ---@type Request
            local req = net.json2lua(message)
            for i = ThingN - NPoints, ThingN do
                trigger.action.removeMark(i)
            end
            for _, point in ipairs(req.Points) do
                ThingN = ThingN + 1
                trigger.action.circleToAll(
                    -1,
                    ThingN,
                    {x = point[1], y = 0, z = point[2]},
                    300,
                    {1,0,0,1},
                    {1,0,0,1},
                    1
                );
            end
            NPoints = #req.Points
            OutText("NPoints: " .. NPoints, 5)
            socket:close()
            SendTest()
        end
    end

    timer.scheduleFunction(
        function()
            local success, err = pcall(PrintAccept)
            if not success then OutText("loop failed: " .. err) end
            return timer.getTime() + 0.2
        end,
        nil, timer.getTime() + 0.2
    )
end

AppendSocketPath()
RunServerLoop()
SendTest()



