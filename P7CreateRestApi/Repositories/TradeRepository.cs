﻿using P7CreateRestApi.Controllers;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using Microsoft.EntityFrameworkCore;


namespace P7CreateRestApi.Repositories
{
    public class TradeRepository : ITradeRepository
    {
        private readonly LocalDbContext _context;

        public TradeRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<Trade> CreateTradeAsync(Trade trade)
        {
            _context.Trades.Add(trade);
            await _context.SaveChangesAsync();
            return trade;
        }

        public async Task<Trade> GetTradeByIdAsync(int id)
        {
            return await _context.Trades.FindAsync(id);
        }

        public async Task<IEnumerable<Trade>> GetAllTradesAsync()
        {
            return await _context.Trades.ToListAsync();
        }

        public async Task<Trade> UpdateTradeAsync(Trade trade)
        {
            _context.Trades.Update(trade);
            await _context.SaveChangesAsync();
            return trade;
        }

        public async Task<bool> DeleteTradeAsync(int id)
        {
            var trade = await _context.Trades.FindAsync(id);
            if (trade == null)
            {
                return false;
            }

            _context.Trades.Remove(trade);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}