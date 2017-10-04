require_relative '../model'

class Customer
  include Model

  service_url "#{ ENV['CUSTOMER_SVC_URL'] || 'http://localhost:9996' }/customers"

  attr_accessor :id,
                :first_name,
                :last_name

  def initialize(args = {})
    args ||= {}
    @id = args[:id]
    @first_name = args[:first_name]
    @last_name = args[:last_name]
  end
end