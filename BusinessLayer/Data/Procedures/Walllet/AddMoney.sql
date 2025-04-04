create or alter procedure AddMoney @amount decimal, @userId int as
begin 
	update wallet  
	set money_for_games = money_for_games + @amount
	where user_id = @userId
end