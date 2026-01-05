/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconBasicUser = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="60"
        height="60"
        viewBox="0 0 60 60"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M22.5 40C18.3579 40 15 43.3579 15 47.5C15 48.8807 16.1193 50 17.5 50H42.5C43.8807 50 45 48.8807 45 47.5C45 43.3579 41.6421 40 37.5 40H22.5ZM10 47.5C10 40.5964 15.5964 35 22.5 35H37.5C44.4036 35 50 40.5964 50 47.5C50 51.6421 46.6421 55 42.5 55H17.5C13.3579 55 10 51.6421 10 47.5Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M30 10C25.8579 10 22.5 13.3579 22.5 17.5C22.5 21.6421 25.8579 25 30 25C34.1421 25 37.5 21.6421 37.5 17.5C37.5 13.3579 34.1421 10 30 10ZM17.5 17.5C17.5 10.5964 23.0964 5 30 5C36.9036 5 42.5 10.5964 42.5 17.5C42.5 24.4036 36.9036 30 30 30C23.0964 30 17.5 24.4036 17.5 17.5Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconBasicUser
